using System;
namespace SharpCpp
{
    public class YReturn : YStatement
    {
        public YExpr Value;

        public YReturn(YExpr value)
        {
            Value = value;
        }
    }
}
