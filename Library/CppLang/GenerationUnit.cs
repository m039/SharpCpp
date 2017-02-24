using System;
using SharpCpp;

namespace SharpCpp
{
    public class GenerationUnit
    {
        public bool IsInterface { get; set; } = false;

        readonly YClass Class;

        readonly TFile _header;

        readonly TFile _source;

        public GenerationUnit(YClass @class)
        {
            Class = @class;

            _header = new TFile {
                Name = Class.Name,
                Type = TFile.TFileType.HEADER
            };

            _source = new TFile {
                Name = Class.Name,
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

        public void Compile(YRoot root)
        {
            _header.Content = new HeaderUnitCompiler().Compile(root, this);
            _source.Content = new SourceUnitCompiler().Compile(root, this);
        }
    }
}
