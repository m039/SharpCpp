using System;
using SharpCpp;

namespace CppLang
{
    public class GenerationUnit
    {
        public string Name;

        public readonly string Namespace;

        public bool IsInterface { get; set; } = false;

        private readonly TFile _header;

        private readonly TFile _source;

        internal YSyntaxNode Root;

        // Note: namespace may be null
        public GenerationUnit(string @namespace, string name)
        {
            Name = name;
            Namespace = @namespace;

            _header = new TFile {
                Name = name,
                Type = TFile.TFileType.HEADER
            };

            _source = new TFile {
                Name = name,
                Type = TFile.TFileType.SOURCE
            };

            // Create AST manually

            Root = new YRoot();
            var _namespaceNode = new YNamespace() {
                Name = "CSharpFooBarLibrary"
            };

            var _classNode = new YClass() {
                Name = "Foo"
            };

            var _fieldNode = new YField() {
                Type = YType.Int,
                Name = "number",
                Value = new YConstExpr(1),
                Visibility = YVisibility.Public
            };

            _classNode.AddChild(_fieldNode);
            _namespaceNode.AddChild(_classNode);
            _namespaceNode.AddChild(_classNode);

            Root.AddChild(_namespaceNode);
        }

        public TFile GeneratedSourceFile()
        {
            return _source;
        }

        public TFile GeneratedHeaderFile()
        {
            return _header;
        }

        public void Compile()
        {
            _header.Content = new HeaderUnitCompiler().Compile(this);
            _source.Content = new SourceUnitCompiler().Compile(this);
        }
    }
}
