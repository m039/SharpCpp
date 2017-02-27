using System;
namespace SharpCpp
{
    public class YFuncSignature : YType
    {
        public YType ReturnType;

        public YVar[] Params;

        public YFuncSignature(string name, YType returnType, YVar[] @params)
        {
            Name = name;
            ReturnType = returnType;
            Params = @params;
        }
    }
}
