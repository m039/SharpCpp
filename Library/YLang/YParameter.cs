using System;
namespace SharpCpp
{
    public class YParameter : YType
    {
        public YType Type;

        public YParameter(string name, YType type)
        {
            Name = name;
            Type = type;
        }
    }
}
