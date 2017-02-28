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

                    var block = new YBlock();

                    foreach (var s in inputMethod.Body.Statements) {
                        if (s is ReturnStatementSyntax) { // return
                            var returnStatement = (ReturnStatementSyntax)s;
                            YExpr returnExpression = null;

                            if (returnStatement.Expression is IdentifierNameSyntax) {
                                returnExpression = new YIdentifierExpr((IdentifierNameSyntax)returnStatement.Expression);
                            } else {
                                //throw new TUnsupportedException();
                            }

                            if (returnExpression != null) {
                                block.Statements.Add(new YReturn(returnExpression));
                            }

                        } else if (s is ExpressionStatementSyntax) { // assignment
                            var expressionStatement = (ExpressionStatementSyntax)s;

                            if (expressionStatement.Expression is AssignmentExpressionSyntax) {
                                var assignmentExpression = (AssignmentExpressionSyntax)expressionStatement.Expression;

                                YExpr left = null;

                                if (assignmentExpression.Left is MemberAccessExpressionSyntax) {
                                    var memberAccessExpression = (MemberAccessExpressionSyntax)assignmentExpression.Left;

                                    YExpr expr = null;

                                    if (memberAccessExpression.Expression is ThisExpressionSyntax) {
                                        expr = YExpr.This;
                                    }

                                    left = new YMemberAccessExpr(
                                        expr,
                                        memberAccessExpression.Name.Identifier.ToString()
                                    );
                                } else if (assignmentExpression.Left is IdentifierNameSyntax) {
                                    left = new YIdentifierExpr((IdentifierNameSyntax)assignmentExpression.Left);
                                }

                                YExpr right = null;

                                if (assignmentExpression.Right is IdentifierNameSyntax) {
                                    right = new YIdentifierExpr((IdentifierNameSyntax)assignmentExpression.Right);
                                }

                                if (left != null && right != null) {
                                    block.Statements.Add(new YAssign(left, right));
                                }
                            } else {
                                //throw new TUnsupportedException();
                            }

                        } else {
                            //throw new TUnsupportedException();
                        }
                    }

                    method.Visibility = inputMethod.Modifiers.GetYVisibility();
                    method.Body = block;
                    @class.AddChild(method);
                }
            }
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
            }

            throw new TException("Unsupported return type");
        }
    }
}
