using System;
namespace SharpCpp
{
    public class YLiteralExpr : YExpr
    {
        public readonly static YLiteralExpr Null = new YLiteralExpr();

        public object Value;

        public YLiteralExpr(object @value)
        {
            Value = @value;
        }

        private YLiteralExpr()
        {
        }
    }
}
