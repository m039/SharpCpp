using System;
namespace SharpCpp
{
    public class YBinaryExpr : YExpr
    {
        public YExpr Left;

        public YExpr Right;

        public YOperator Operator;

        public YBinaryExpr(YExpr left, YExpr right, YOperator operation)
        {
            Left = left;
            Right = right;
            Operator = operation;
        }
    }
}
