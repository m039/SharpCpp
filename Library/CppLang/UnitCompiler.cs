using System;
using SharpCpp;

namespace SharpCpp
{
    public abstract class UnitCompiler
    {
        public abstract string Compile(YRoot root, GenerationUnit unit);
    }
}
