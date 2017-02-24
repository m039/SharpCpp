using System;
using System.Collections.Generic;

namespace SharpCpp
{
    public class YRoot : YSyntaxNode
    {
        private readonly List<YSyntaxNode> _Namespaces = new List<YSyntaxNode>();

        public void AddChild(YSyntaxNode node)
        {
            _Namespaces.Add(node);
        }

        public YNamespace FindNamespaceByName(string name)
        {
            return _Namespaces.Find((obj) => (obj as YNamespace)?.Name == name) as YNamespace;
        }

        public IEnumerable<YSyntaxNode> Nodes {
            get {
                return _Namespaces;
            }
        }
    }
}
