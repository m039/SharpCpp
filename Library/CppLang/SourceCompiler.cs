using System;
using System.Text;
using System.Collections.Generic;

namespace SharpCpp
{
    public class SourceUnitCompiler : UnitCompiler
    {
        class SourceWalker : UnitWalker
        {
            protected const string ConstructorsMark = "{{constructors}}";

            StringBuilder _constructor = new StringBuilder();

            bool _constructorInited;

            readonly TypeMapper _typeMapper;

            public SourceWalker(YClass @class, TypeMapper.IncludeFinder finder) : base(@class, finder) {
                _typeMapper = new TypeMapper(finder, new HashSet<string>()); // ignore includes for now
            }

            internal protected override void InitBuilder(StringBuilder builder)
            {
                base.InitBuilder(builder);

                _constructor.Clear();
                _constructor.Append($"{ Class.Name }::{ Class.Name }()");
                _constructorInited = true;

                builder.Append($"#include \"{ Class.Name }.hpp\"\n");
                builder.Append(IncludesMark);
                builder.Append("\n");
                builder.Append(ConstructorsMark);
            }

            internal protected override void Visit(StringBuilder builder, YNamespace @namespace)
            {
                builder.Replace(
                   ConstructorsMark,
                   "using namespace " + @namespace.Name + ";\n\n" + ConstructorsMark
                );
            }

            internal protected override void Visit(StringBuilder builder, YField field)
            {
                base.Visit(builder, field);

                if (field.Value is YConstExpr) {
                    if (_constructorInited) {
                        _constructor.Append(":");
                        _constructorInited = false;
                    } else {
                        _constructor.Append(",");
                    }

                    _constructor.Append(field.Name + "{" + field.Value + "}");
                }
            }

            internal protected override void Visit(StringBuilder builder, YMethod method)
            {
                base.Visit(builder, method);

                if (method.IsPure) {
                    return;
                }

                if (method is YDestructor) {
                    builder.Append($"{ Class.Name }::~{ Class.Name }()");
                } else {
                    builder.Append(_typeMapper.ValueOf(method.Signature.ReturnType));
                    builder.Append(" ");
                    builder.Append($"{ Class.Name }::{ method.Name }");

                    builder.Append("(");
                    builder.Append(_typeMapper.ValueOf(method.Signature.Parameters));
                    builder.Append(")");
                }

                builder.AppendEx(method.Body);
            }

            internal protected override void FinalizeBuilder(StringBuilder builder)
            {
                base.FinalizeBuilder(builder);

                builder.Replace(IncludesMark, "");

                // only default constructor supported
                _constructor.Append("{}");

                builder.Replace(ConstructorsMark, _constructor.ToString());
            }
        }

        public override UnitWalker CreateUnitWalker(YClass @class, TypeMapper.IncludeFinder finder)
        {
            return new SourceWalker(@class, finder);
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Unwrap statement to string.
        /// </summary>
        static public void AppendEx(this StringBuilder builder, YStatement statement)
        {
            if (statement is YBlock) {
                var block = (YBlock)statement;

                builder.Append("{");

                foreach (var s in block.Statements) {
                    AppendEx(builder, s);
                }

                builder.Append("}");
            } else if (statement is YReturn) {
                var @return = (YReturn)statement;

                builder.Append("return ");
                AppendEx(builder, @return.Value);
                builder.Append(";");

            } else if (statement is YAssign) {
                var assign = (YAssign)statement;

                AppendEx(builder, assign.Left);
                builder.Append("=");
                AppendEx(builder, assign.Right);
                builder.Append(";");
            } else if (statement is YIf) {
                var @if = (YIf)statement;

                builder.Append("if (");
                AppendEx(builder, @if.Condition);
                builder.Append(") ");
                AppendEx(builder, @if.Statement);
                if (@if.ElseStatement != null) {
                    builder.Append(" else ");
                    AppendEx(builder, @if.ElseStatement);
                }
            }
        }

        /// <summary>
        /// Unwrap expression to string.
        /// </summary>
        static public void AppendEx(this StringBuilder builder, YExpr expr)
        {
            if (expr is YConstExpr) {
                builder.Append("" + expr);
            } else if (expr is YThisExpr) {
                builder.Append("this");
            } else if (expr is YMemberAccessExpr) {
                var memberAccess = (YMemberAccessExpr)expr;

                AppendEx(builder, memberAccess.Expression);
                builder.Append("->"); // But what with "this."?
                builder.Append(memberAccess.Name);
            } else if (expr is YIdentifierExpr) {
                builder.Append(((YIdentifierExpr)expr).Name);
            } else if (expr is YBinaryExpr) {
                var binary = (YBinaryExpr)expr;

                AppendEx(builder, binary.Left);
                builder.Append(" ");
                AppendEx(builder, binary.Operator);
                builder.Append(" ");
                AppendEx(builder, binary.Right);
            } else if (expr is YLiteralExpr) {
                var literal = (YLiteralExpr)expr;
                if (literal == YLiteralExpr.Null) {
                    builder.Append("nullptr");
                } else {
                    builder.Append(literal.Value);
                }
            } else if (expr is YPrefixUnaryExpr) {
                var prefixUnari = (YPrefixUnaryExpr)expr;

                AppendEx(builder, prefixUnari.Operator);
                AppendEx(builder, prefixUnari.Operand);
            } else if (expr is YInvocation) {
                var invocation = (YInvocation)expr;

                AppendEx(builder, invocation.Expression);

                // no arguments
                builder.Append("()");
            }
        }

        static public void AppendEx(this StringBuilder builder, YOperator op)
        {
            if (op == YOperator.EqualsEquals) {
                builder.Append("==");
            } else if (op == YOperator.Minus) {
                builder.Append("-");
            }
        }
    }
}
