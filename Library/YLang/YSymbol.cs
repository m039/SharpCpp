using System;
using System.Collections.Generic;
using SharpCpp;

namespace SharpCpp
{
    public class YSymbol : YSyntaxNode
    {
        public virtual string Name { get; set; }

        readonly List<YSyntaxNode> _Nodes = new List<YSyntaxNode>();

        public virtual void AddChild(YSyntaxNode node)
        {
            _Nodes.Add(node);
        }

        public bool ChildOf(YSymbol symbol)
        {
            // Performance note: O(n) where n is the number of childs!
            foreach (var node in symbol.Nodes) {
                if (node == this)
                    return true;
            }

            return false;
        }

        public bool ParentOf(YSymbol symbol)
        {
            // Note: O(n)!

            foreach (var node in Nodes) {
                if (node == symbol)
                    return true;
            }

            return false;
        }

        public IEnumerable<YSyntaxNode> Nodes { get { return _Nodes; } }
    }
}
