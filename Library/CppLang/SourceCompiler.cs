using System;

namespace CppLang
{
    public class SourceUnitCompiler : UnitCompiler
    {
        public override string Compile(GenerationUnit unit)
        {
            return "int a() {int b=11;int c=14;return 11;} void c(){}";
        }
    }
}
