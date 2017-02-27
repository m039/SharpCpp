using System;
namespace SharpCpp
{
    public class YFieldAccessExpr : YExpr
    {
        public YField Field;

        public YFieldAccessExpr(YField field)
        {
            Field = field;
        }
    }
}
