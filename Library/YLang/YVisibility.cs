using System;
namespace SharpCpp
{
    public enum YVisibility
    {
        Private = 0,
        Public = 1,
    }

    public static partial class Extensions
    {
        public static string GetName(this YVisibility visibility)
        {
            switch (visibility) {
                case YVisibility.Private:
                    return "private";
                case YVisibility.Public:
                    return "public";
                default:
                    throw new Exception("Bad type");
            }
        }
    }
}
