using System;
namespace SharpCpp
{
    public class YMethod : YSymbol
    {
        #region vars

        YStatement _body;

        bool _isVirtual;

        #endregion

        public YMethod(string name) : this(name, YType.Void)
        {
        }

        public YMethod(string name, YType type) : this (name, type, null)
        {
        }

        public YMethod(string name, YType returnType, YParameter[] @params)
        {
            Signature = new YFuncSignature(name, returnType, @params);
        }

        public override string Name {
            get {
                return Signature.Name;
            }
            set {
                Signature.Name = value;
            }
        }

        public YFuncSignature Signature;

        public YVisibility Visibility;

        public YStatement Body {
            get {
                if (IsPure) {
                    throw new TException("Pure method doesn't have a body");
                }

                return _body;
            } 
            set {
                _body = value;
                IsPure = false;
            }
        }

        public bool IsPure;

        public bool IsVirtual {
            get {
                if (IsPure) {
                    return true;
                }

                return _isVirtual;
            }
            set {
                _isVirtual = value;
            }
        }
    }
}
