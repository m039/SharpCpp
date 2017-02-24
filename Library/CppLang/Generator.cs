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

        public TFile[] Generate(SyntaxNode input)
        {
            YRoot root = new YRoot();

            // Namespaces
            foreach (var inputNamespace in input.ChildNodes().OfType<NamespaceDeclarationSyntax>()) {
                var @namespace = new YNamespace(inputNamespace.Name.ToString());
                root.AddChild(@namespace);

                // Classes
                foreach (var inputClass in inputNamespace.ChildNodes().OfType<ClassDeclarationSyntax>()) {
                    var @class = new YClass(inputClass.Identifier.ToString());
                    root.AddChild(@class);

                    _generationUnits.Add(new GenerationUnit(@class));

                    // Fields
                    foreach (var inputField in inputClass.ChildNodes().OfType<FieldDeclarationSyntax>()) {
                        var modifiers = inputField.Modifiers;
                        var declaration = inputField.Declaration;
                        var declarationType = declaration.Type;
                        var variables = declaration.Variables;

                        if (variables.Count == 1 && 
                            declarationType.IsKind(SyntaxKind.PredefinedType)) {
                            var predefinedType = (PredefinedTypeSyntax)declarationType;

                            if (predefinedType.Keyword.IsKind(SyntaxKind.IntKeyword)) {
                                var field = new YField() {
                                    Type = YType.Int,
                                    Name = variables[0].Identifier.ToString()
                                };

                                if (modifiers.Any(SyntaxKind.PublicKeyword)) {
                                    field.Visibility = YVisibility.Public;
                                }

                                @class.AddChild(field);
                            } else {
                                throw new TException("Unsupported");
                            }
                        } else {
                            throw new TException("Unsupported");
                        }
                    }
                }
            }

            List<TFile> files = new List<TFile>();

            foreach (var unit in _generationUnits) {
                unit.Compile(root);
                files.Add(unit.GeneratedSourceFile());
                files.Add(unit.GeneratedHeaderFile());
            }

            return files.ToArray();
        }
    }
}
