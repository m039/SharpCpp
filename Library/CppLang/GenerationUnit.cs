using System;
using SharpCpp;

namespace SharpCpp
{
    public class GenerationUnit
    {
        public readonly YClass Class;

        readonly GeneratedFile _header;

        readonly GeneratedFile _source;

        public GenerationUnit(YClass @class)
        {
            Class = @class;

            _header = new GeneratedFile {
                Name = Class.Name,
                Type = GeneratedFile.GeneratedFileType.HEADER
            };

            _source = new GeneratedFile {
                Name = Class.Name,
                Type = GeneratedFile.GeneratedFileType.SOURCE
            };
        }

        public GeneratedFile GeneratedSourceFile()
        {
            return _source;
        }

        public GeneratedFile GeneratedHeaderFile()
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
