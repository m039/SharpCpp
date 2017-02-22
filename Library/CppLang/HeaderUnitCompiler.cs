using System;
namespace CppLang
{
	public class HeaderUnitCompiler : UnitCompiler
	{
		public override string Compile(GenerationUnit unit)
		{
			return "class " + unit.Name + " {};";
		}
	}
}
