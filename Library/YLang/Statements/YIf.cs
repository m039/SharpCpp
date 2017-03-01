using System;
namespace SharpCpp
{
    public class YIf : YStatement
    {
        public YExpr Condition;

        public YStatement Statement;

        public YStatement ElseStatement;

        public YIf(YExpr condition, YStatement statement, YStatement elseStatement)
        {
            Condition = condition;
            Statement = statement;
            ElseStatement = elseStatement;
        }
    }
}
