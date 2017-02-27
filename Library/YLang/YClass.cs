using System;
namespace SharpCpp
{
    public class YClass : YSymbol
    {
        // hack: nested classes are not supported
        public bool IsNested;

        public YClass(string name)
        {
            Name = name;
        }
    }
}
