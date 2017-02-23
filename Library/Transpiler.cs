using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpCpp
{
    public static class Transpiler
    {
        public static TFile[] compileCSharpToCpp(string code)
        {
            if (code == null || "".Equals(code)) {
                throw new TException("What are you doing!?");
            }

            var walker = new TSyntaxWalker();

            walker.Visit(CSharpSyntaxTree.ParseText(code).GetRoot());

            return Prettify(walker.GetGeneratedFiles());
        }

        private static TFile[] Prettify(TFile[] generatedFiles)
        {
            using (var prettifier = new Prettifier()) {
                foreach (var file in generatedFiles) {
                    prettifier.Prettify(file);
                }

                return generatedFiles;
            }
        }
    }
}
