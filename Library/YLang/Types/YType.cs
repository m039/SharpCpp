using System;
namespace SharpCpp
{
    public class YType : YSymbol
    {
        public static readonly YType Int = new YType();

        public static readonly YType Void = new YType();

        public static readonly YType NoType = null; // null is intended
    }
}
