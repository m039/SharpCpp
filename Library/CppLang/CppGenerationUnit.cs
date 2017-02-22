using System;
namespace CSharpCpp
{
	public class CppGenerationUnit
	{
		public string Name;

		public readonly string Namespace;

		public bool IsInterface { get; set; } = false;

		private readonly TFile _header;

		private readonly TFile _source;

		// Note: namespace may be null
		public CppGenerationUnit(string @namespace, string name)
		{
			Name = name;

			_header = new TFile
			{
				Name = name,
				Type = TFile.TFileType.HEADER
			};

			_source = new TFile
			{
				Name = name,
				Type = TFile.TFileType.SOURCE
			};
		}

		public TFile GeneratedSourceFile()
		{
			return _source;
		}

		public TFile GeneratedHeaderFile()
		{
			return _header;
		}

		public void Compile()
		{
			_header.Content = "header class " + Name + " " + IsInterface;
			_source.Content = "source class " + Name + " " + IsInterface;
		}
	}
}
