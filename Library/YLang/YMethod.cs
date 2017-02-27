using System;
namespace SharpCpp
{
    public class YMethod : YSymbol
    {
        public override string Name {
            get {
                return Signature.Name;
            }
            set {
                Signature.Name = value;
            }
        }

        public YMethod(string name) : this(name, YType.Void)
        {
        }

        public YMethod(string name, YType type) : this (name, type, null)
        {
        }

        public YMethod(string name, YType returnType, YVar[] @params)
        {
            Signature = new YFuncSignature(name, returnType, @params);
        }

        public YClass Class;

        public YFuncSignature Signature;

        public YVisibility Visibility;
    }
}
