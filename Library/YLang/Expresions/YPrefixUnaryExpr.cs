using System;
namespace SharpCpp
{
    public class YPrefixUnaryExpr : YExpr
    {
        public YOperator Operator;

        public YExpr Operand;

        public YPrefixUnaryExpr(YOperator @operator, YExpr operand)
        {
            Operator = @operator;
            Operand = operand;
        }
    }
}
