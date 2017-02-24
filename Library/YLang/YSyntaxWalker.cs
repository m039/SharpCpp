using System;
namespace SharpCpp
{
    public class YSyntaxWalker
    {
        protected virtual void Visit(YNamespace @namespace) { }
   
        protected virtual void Visit(YClass @class) { }

        protected virtual void Visit(YField @field) { }

        public void Walk(YSyntaxNode node)
        {
            if (node is YNamespace) {
                Visit((YNamespace)node);
            } else if (node is YClass) {
                Visit((YClass)node);
            } if (node is YField) {
                Visit((YField)node);
            }

            foreach (var n in node.Nodes) {
                Walk(n);
            }
        }
    }
}
