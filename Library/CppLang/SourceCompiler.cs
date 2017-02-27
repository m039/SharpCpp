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

            public SourceWalker(YClass @class) : base(@class) { }

            StringBuilder _constructor = new StringBuilder();

            bool _constructorInited;

            TypeMapper _typeMapper = new TypeMapper(new HashSet<string>()); // ignore includes for now

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

                builder.Append(_typeMapper.ValueOf(method.Signature.ReturnType));
                builder.Append(" ");
                builder.Append($"{ Class.Name }::{ method.Name }");

                builder.Append("(");
                builder.Append(_typeMapper.ValueOf(method.Signature.Parameters));
                builder.Append(")");

                Append(builder, method.Body);
            }

            static void Append(StringBuilder builder, YStatement statement)
            {
                if (statement is YBlock) {
                    var block = (YBlock)statement;

                    builder.Append("{");

                    foreach (var s in block.Statements) {
                        Append(builder, s);
                    }

                    builder.Append("}");
                } else if (statement is YReturn) {
                    var @return = (YReturn)statement;

                    builder.Append("return ");
                    Append(builder, @return.Value);
                    builder.Append(";");

                } else if (statement is YAssign) {
                    var assign = (YAssign)statement;

                    Append(builder, assign.Left);
                    builder.Append("=");
                    Append(builder, assign.Right);
                    builder.Append(";");
                }
            }

            static void Append(StringBuilder builder, YExpr expr)
            {
                if (expr is YConstExpr) {
                    builder.Append("" + expr);
                } else if (expr is YThisExpr) {
                    builder.Append("this");
                } else if (expr is YMemberAccessExpr) {
                    var memberAccess = (YMemberAccessExpr)expr;

                    Append(builder, memberAccess.Expression);
                    builder.Append("->"); // But what with "this."?
                    builder.Append(memberAccess.Name);
                } else if (expr is YIdentifierExpr) {
                    builder.Append(((YIdentifierExpr)expr).Name);
                }
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

        public override UnitWalker CreateUnitWalker(YClass @class)
        {
            return new SourceWalker(@class);
        }
    }
}
