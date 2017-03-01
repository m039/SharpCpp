using System;
namespace SharpCpp
{
    public class YExprStatement : YStatement
    {
        public YExpr Expression;

        public YExprStatement(YExpr expression)
        {
            Expression = expression;
        }
    }
}
