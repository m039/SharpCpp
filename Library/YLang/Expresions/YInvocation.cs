using System;
namespace SharpCpp
{
    public class YInvocation : YExpr
    {
        // no arguments invocation

        public YExpr Expression;

        public YInvocation(YExpr expression)
        {
            Expression = expression;
        }
    }
}
