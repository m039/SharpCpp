using System;
namespace SharpCpp
{
    public class TFile
    {
        public enum TFileType
        {
            HEADER,
            SOURCE
        }

        public string Name { get; set; }

        public TFileType Type { get; set; }

        public string Content { get; set; }

        public string FileExtension()
        {
            switch (Type) {
                case TFileType.HEADER:
                    return "hpp";
                case TFileType.SOURCE:
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
