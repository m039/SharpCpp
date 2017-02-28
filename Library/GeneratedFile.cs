using System;
namespace SharpCpp
{
    public class GeneratedFile
    {
        public enum GeneratedFileType
        {
            HEADER,
            SOURCE
        }

        public string Name { get; set; }

        public GeneratedFileType Type { get; set; }

        public string Content { get; set; }

        public string FileExtension()
        {
            switch (Type) {
                case GeneratedFileType.HEADER:
                    return "hpp";
                case GeneratedFileType.SOURCE:
                    return "cpp";
                default:
                    throw new Exception();
            }
        }

        public override string ToString()
        {
            return $"{Name}.{FileExtension()}";
        }
    }
}
