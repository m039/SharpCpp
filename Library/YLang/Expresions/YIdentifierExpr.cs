using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpCpp
{
    public class YIdentifierExpr : YExpr
    {
        public string Name;

        public YIdentifierExpr(IdentifierNameSyntax identifierNameSyntax)
            : this(identifierNameSyntax.Identifier.ToString())
        {
            
        }

        public YIdentifierExpr(string name)
        {
            Name = name;
        }
    }
}
