using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpCpp
{
	public class TSyntaxWalker : CSharpSyntaxWalker
	{
		private const string Separator = ":";

		private readonly Dictionary<string, CppGenerationUnit> _generationUnits = new Dictionary<string, CppGenerationUnit>();

		private class WalkerState
		{
			// Doesn't support nested namespaces
			public string currentNamespace;
		}

		private WalkerState _walkerState;

		#region CSharpSyntaxWalker

		public override void Visit(Microsoft.CodeAnalysis.SyntaxNode node)
		{
			_walkerState = new WalkerState();
			base.Visit(node);
		}

		public override void VisitNamespaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);

			_walkerState.currentNamespace = node.Name.ToString();
		}

		public override void VisitClassDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);

			CreateGenerationUnitsIfNotExist(node.Identifier.ToString(), false);
		}

		public override void VisitInterfaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax node)
		{
			base.VisitInterfaceDeclaration(node);

			CreateGenerationUnitsIfNotExist(node.Identifier.ToString(), true);
		}

		#endregion

		private void CreateGenerationUnitsIfNotExist(string className, bool isInterface)
		{
			var classFullName = GetClassFullName(_walkerState, className);

			if (!_generationUnits.ContainsKey(classFullName))
			{
				var generationUnit = new CppGenerationUnit(_walkerState.currentNamespace, className);;
				_generationUnits[classFullName] = generationUnit;
				generationUnit.IsInterface = isInterface;
			}
		}

		private static string GetClassFullName(WalkerState walkerState, string name)
		{
			if (walkerState.currentNamespace != null)
			{
				return walkerState.currentNamespace + Separator + name;
			}
			else
			{
				return name;
			}
		}

		public TFile[] GetGeneratedFiles()
		{
			List<TFile> files = new List<TFile>();

			foreach (var unit in _generationUnits.Values) {
				unit.Compile();
				files.Add(unit.GeneratedSourceFile());
				files.Add(unit.GeneratedHeaderFile());
			}

			return files.ToArray();
		}
	}
}
