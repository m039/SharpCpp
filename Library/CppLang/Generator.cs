using System;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace SharpCpp
{
    public class Generator
    {
        List<GenerationUnit> _generationUnits = new List<GenerationUnit>();

        public GeneratedFile[] Generate(SyntaxNode input)
        {
            YRoot root = new YRoot();

            // Namespaces:

            foreach (var inputNamespace in input.ChildNodes().OfType<NamespaceDeclarationSyntax>()) {
                var @namespace = new YNamespace(inputNamespace.GetName());
                root.AddChild(@namespace);

                ProcessInterfaces(@namespace, inputNamespace);
                ProcessClasses(@namespace, inputNamespace);
            }

            List<GeneratedFile> files = new List<GeneratedFile>();

            foreach (var unit in _generationUnits) {
                unit.Compile(root);
                files.Add(unit.GeneratedSourceFile());
                files.Add(unit.GeneratedHeaderFile());
            }

            return files.ToArray();
        }

        void ProcessInterfaces(YNamespace @namespace, NamespaceDeclarationSyntax inputNamespace)
        {
            // Interfaces:

            foreach (var inputInterace in inputNamespace.ChildNodes().OfType<InterfaceDeclarationSyntax>()) {
                var interfaceName = inputInterace.GetName();
                if (interfaceName == "")
                    continue;

                var @interface = new YClass(interfaceName);
                @namespace.AddChild(@interface);

                _generationUnits.Add(new GenerationUnit(@interface));

                // Methods:

                foreach (var inputMethod in inputInterace.ChildNodes().OfType<MethodDeclarationSyntax>()) {
                    var name = inputMethod.Identifier.ToString();
                    var methodReturnType = inputMethod.ReturnType;
                    var parameterList = inputMethod.ParameterList;
                    YMethod method;

                    // Signature:

                    if (parameterList.Parameters.Count() <= 0) {
                        method = new YMethod(name, inputMethod.ReturnType.GetYType());
                    } else {
                        var @params = new List<YParameter>();

                        foreach (var p in parameterList.Parameters) {
                            @params.Add(new YParameter(p.GetName(), p.Type.GetYType()));
                        }

                        method = new YMethod(name, inputMethod.ReturnType.GetYType(), @params.ToArray());
                    }

                    // Body:

                    // all methods of an interface are public and pure by default
                    method.IsPure = true;
                    method.Visibility = YVisibility.Public;
                    @interface.AddChild(method);
                }

                // Misc:

                var destructor = new YDestructor();

                destructor.IsVirtual = true;
                destructor.Visibility = YVisibility.Public;
                destructor.Body = new YBlock();

                @interface.AddChild(destructor);
            }
        }

        void ProcessClasses(YNamespace @namespace, NamespaceDeclarationSyntax inputNamespace)
        {
            // Classes:

            foreach (var inputClass in inputNamespace.ChildNodes().OfType<ClassDeclarationSyntax>()) {
                var className = inputClass.GetName();
                if (className == "")
                    continue;

                var @class = new YClass(className);
                @namespace.AddChild(@class);

                _generationUnits.Add(new GenerationUnit(@class));

                // Fields:

                foreach (var inputField in inputClass.ChildNodes().OfType<FieldDeclarationSyntax>()) {
                    var declaration = inputField.Declaration;
                    var declarationType = declaration.Type;
                    var variables = declaration.Variables;

                    if (variables.Count == 1) {
                        var variable = variables[0];

                        var field = new YField() {
                            Type = declarationType.GetYType(),
                            Name = variable.GetName()
                        };

                        field.Visibility = inputField.Modifiers.GetYVisibility();

                        // expresions: literal

                        // todo process negative numbers

                        if (variable.Initializer?.Value is LiteralExpressionSyntax) {
                            var literalExperssion = (LiteralExpressionSyntax)variable.Initializer.Value;

                            if (literalExperssion.Token.Value is int) {
                                field.Value = new YConstExpr((int)literalExperssion.Token.Value);
                            }
                        }

                        @class.AddChild(field);
                    } else {
                        throw new TException("Unsupported");
                    }
                }

                // Methods:

                foreach (var inputMethod in inputClass.ChildNodes().OfType<MethodDeclarationSyntax>()) {
                    var name = inputMethod.Identifier.ToString();
                    var methodReturnType = inputMethod.ReturnType;
                    var parameterList = inputMethod.ParameterList;
                    YMethod method;

                    // Signature:

                    if (parameterList.Parameters.Count() <= 0) {
                        method = new YMethod(name, inputMethod.ReturnType.GetYType());
                    } else {
                        var @params = new List<YParameter>();

                        foreach (var p in parameterList.Parameters) {
                            @params.Add(new YParameter(p.GetName(), p.Type.GetYType()));
                        }

                        method = new YMethod(name, inputMethod.ReturnType.GetYType(), @params.ToArray());
                    }

                    // Body:

                    method.Visibility = inputMethod.Modifiers.GetYVisibility();
                    method.Body = ProcessStatement(inputMethod.Body);
                    @class.AddChild(method);
                }
            }
        }

        // todo here could be generics or reflection

        YStatement ProcessStatement(StatementSyntax statement)
        {
            if (statement is IfStatementSyntax) {
                return ProcessStatement((IfStatementSyntax)statement);
            } else if (statement is BlockSyntax) {
                return ProcessStatement((BlockSyntax)statement);
            } else if (statement is ReturnStatementSyntax) {
                return ProcessStatement((ReturnStatementSyntax)statement);
            } else if (statement is ExpressionStatementSyntax) {
                return ProcessStatement((ExpressionStatementSyntax)statement);
            }

            throw new TException("Unable to process statement");
        }

        YExpr ProcessExpr(ExpressionSyntax expr)
        {
            if (expr is BinaryExpressionSyntax) {
                return ProcessExpr((BinaryExpressionSyntax)expr);
            } else if (expr is IdentifierNameSyntax) {
                return ProcessExpr((IdentifierNameSyntax)expr);
            } else if (expr is LiteralExpressionSyntax) {
                return ProcessExpr((LiteralExpressionSyntax)expr);
            } else if (expr is PrefixUnaryExpressionSyntax) {
                return ProcessExpr((PrefixUnaryExpressionSyntax)expr);
            } else if (expr is InvocationExpressionSyntax) {
                return ProcessExpr((InvocationExpressionSyntax)expr);
            } else if (expr is MemberAccessExpressionSyntax) {
                return ProcessExpr((MemberAccessExpressionSyntax)expr);
            } else if (expr is ThisExpressionSyntax) {
                return ProcessExpr((ThisExpressionSyntax)expr);
            }

            throw new TException("Unable to process expression");
        }

        YExpr ProcessExpr(IdentifierNameSyntax expr)
        {
            return new YIdentifierExpr(expr);
        }

        YExpr ProcessExpr(LiteralExpressionSyntax expr)
        {
            if (expr.Token.IsKind(SyntaxKind.NullKeyword)) {
                return YLiteralExpr.Null;
            } else if (expr.Token.IsKind(SyntaxKind.NumericLiteralToken)) {
                return new YLiteralExpr(expr.Token.Value); // looks same as YConstExpr
            }

            throw new TException("Unable to process expr");
        }

        YExpr ProcessExpr(PrefixUnaryExpressionSyntax expr)
        {
            var operand = ProcessExpr(expr.Operand);
            var @operator = ProcessOperator(expr.OperatorToken);

            return new YPrefixUnaryExpr(@operator, operand);
        }

        YExpr ProcessExpr(MemberAccessExpressionSyntax memberAccessExpression)
        {
            YExpr expr = ProcessExpr(memberAccessExpression.Expression);

            return new YMemberAccessExpr(
                expr,
                memberAccessExpression.Name.Identifier.ToString()
            );
        }

        YExpr ProcessExpr(ThisExpressionSyntax expr)
        {
            return YExpr.This;
        }

        YExpr ProcessExpr(InvocationExpressionSyntax expr)
        {
            if (expr.ArgumentList.ChildNodes().Count() > 0) {
                throw new TException("Unable to process expression");
            }

            return new YInvocation(ProcessExpr(expr.Expression));
        }

        YExpr ProcessExpr(BinaryExpressionSyntax binaryExpression)
        {
            var left = ProcessExpr(binaryExpression.Left);
            var right = ProcessExpr(binaryExpression.Right);
            var operation = ProcessOperator(binaryExpression.OperatorToken);

            return new YBinaryExpr(left, right, operation);
        }

        YStatement ProcessStatement(IfStatementSyntax ifStatement)
        {
            YExpr condition = ProcessExpr(ifStatement.Condition);
            YStatement statement = ProcessStatement(ifStatement.Statement);
            YStatement elseStatement = ProcessStatement(ifStatement.Else.Statement);

            if (condition != null && statement != null) {
                return new YIf(condition, statement, elseStatement);
            }

            throw new TException("Unable to process statement");
        }

        YStatement ProcessStatement(ReturnStatementSyntax statement)
        {
            return new YReturn(ProcessExpr(statement.Expression));
        }

        YStatement ProcessStatement(BlockSyntax statement)
        {
            var block = new YBlock();

            foreach (var s in statement.Statements) {
                block.Statements.Add(ProcessStatement(s));
            }

            return block;
        }

        YStatement ProcessStatement(ExpressionStatementSyntax statement)
        {
            if (statement.Expression is AssignmentExpressionSyntax) { // ?!
                var assignmentExpression = (AssignmentExpressionSyntax)statement.Expression;

                YExpr left = ProcessExpr(assignmentExpression.Left);
                YExpr right = ProcessExpr(assignmentExpression.Right);
                             
                if (left != null && right != null) {
                    return new YAssign(left, right); // expression or statement?!
                }
            }

            throw new TException("Unable to process statement");
        }
      
        YOperator ProcessOperator(SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.EqualsEqualsToken)) {
                return YOperator.EqualsEquals;
            } else if (token.IsKind(SyntaxKind.MinusToken)) {
                return YOperator.Minus;
            }

            throw new TException("Unable to process opeartion");
        }
    }

    static partial class Extensions
    {
        static public string GetName(this NamespaceDeclarationSyntax syntax)
        {
            return syntax.Name.ToString();
        }

        static public string GetName(this ClassDeclarationSyntax syntax)
        {
            return syntax.Identifier.ToString();
        }

        static public string GetName(this InterfaceDeclarationSyntax syntax)
        {
            return syntax.Identifier.ToString();
        }

        static public string GetName(this VariableDeclaratorSyntax syntax)
        {
            return syntax.Identifier.ToString();
        }

        static public string GetName(this ParameterSyntax syntax)
        {
            return syntax.Identifier.ToString();
        }

        static public string GetName(this IdentifierNameSyntax syntax)
        {
            return syntax.Identifier.ToString();
        }

        static public YVisibility GetYVisibility(this SyntaxTokenList modifiers)
        {
            if (modifiers.Any(SyntaxKind.PublicKeyword)) {
                return YVisibility.Public;
            } else {
                return YVisibility.Private;
            }
        }

        static public YType GetYType(this TypeSyntax typeSyntax)
        {
            if (typeSyntax.IsKind(SyntaxKind.PredefinedType)) {
                var predefinedTypeKeywoard = ((PredefinedTypeSyntax)typeSyntax).Keyword;

                if (predefinedTypeKeywoard.IsKind(SyntaxKind.IntKeyword)) {
                    return YType.Int;
                } else if (predefinedTypeKeywoard.IsKind(SyntaxKind.VoidKeyword)) {
                    return YType.Void;
                }
            } else if (typeSyntax.IsKind(SyntaxKind.IdentifierName)) {
                var identifierToken = (IdentifierNameSyntax)typeSyntax;

                // other types are references.. for now
                return new YRefType(identifierToken.GetName());
            }

            throw new TException("Unsupported return type");
        }
    }
}
