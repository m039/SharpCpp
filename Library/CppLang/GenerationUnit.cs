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
