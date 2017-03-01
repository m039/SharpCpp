using System;
namespace SharpCpp
{
    public class YAssign : YExpr
    {
        public YExpr Left;
        public YExpr Right;

        public YAssign(YExpr left, YExpr right)
        {
            Left = left;
            Right = right;
        }
    }
}
