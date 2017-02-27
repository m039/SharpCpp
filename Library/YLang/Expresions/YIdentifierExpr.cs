using System;
namespace SharpCpp
{
    public class YIdentifierExpr : YExpr
    {
        public string Name;

        public YIdentifierExpr(string name)
        {
            Name = name;
        }
    }
}
