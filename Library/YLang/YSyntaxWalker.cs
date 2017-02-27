using System;
namespace SharpCpp
{
    public class YSyntaxWalker
    {
        object guardNode;

        protected virtual void Visit(YNamespace @namespace) { }
   
        protected virtual void Visit(YClass @class) { }

        protected virtual void Visit(YField @field) { }

        protected virtual void Visit(YMethod @method) { }

        protected virtual void OnPreWalk() { }

        protected virtual void OnPostWalk() { }

        public void Walk(YSyntaxNode node)
        {
            if (guardNode == null) {
                guardNode = node;
                OnPreWalk();
            }

            if (node is YNamespace) {
                Visit((YNamespace)node);
            } else if (node is YClass) {
                Visit((YClass)node);
            } else if (node is YMethod) {
                Visit((YMethod)node);
            } else if (node is YField) {
                Visit((YField)node);
            }

            foreach (var n in node.Nodes) {
                Walk(n);
            }

            if (guardNode == node) {
                OnPostWalk();
                guardNode = null;
            }
        }
    }
}
