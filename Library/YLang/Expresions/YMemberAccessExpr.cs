using System;
namespace SharpCpp
{
    public class YMemberAccessExpr : YExpr
    {
        public YExpr Expression;
        public string Name;

        public YMemberAccessExpr(YExpr expression, string name)
        {
            Expression = expression;
            Name = name;
        }
    }
}
