using System;
namespace SharpCpp
{
    public class YField : YSymbol
    {
        public YType Type { get; set; }

        public YExpr Value { get; set; }

        public YVisibility Visibility { get; set; }
    }
}
