using System;
namespace SharpCpp
{
    public class YConstExpr : YExpr
    {
        public object Value;

        public YConstExpr(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}
