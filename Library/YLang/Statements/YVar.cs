using System;
namespace SharpCpp
{
    public class YVar : YSymbol, YStatement
    {
        public YVar(string name)
        {
            Name = name;
        }
    }
}
