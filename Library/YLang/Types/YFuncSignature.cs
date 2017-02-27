using System;
namespace SharpCpp
{
    public class YFuncSignature : YType
    {
        public YType ReturnType;

        public YParameter[] Parameters;

        public YFuncSignature(string name, YType returnType, YParameter[] @params)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = @params;
        }
    }
}
