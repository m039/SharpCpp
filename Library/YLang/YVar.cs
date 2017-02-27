using System;
namespace SharpCpp
{
    public class YVar : YSymbol
    {
        public YType Type;

        public YVar(string name, YType type)
        {
            Name = name;
            Type = type;
        }
    }
}
