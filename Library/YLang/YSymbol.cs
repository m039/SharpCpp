﻿using System;
using System.Collections.Generic;
using SharpCpp;

namespace SharpCpp
{
    public class YSymbol : YSyntaxNode
    {
        public string Name;

        private readonly List<YSyntaxNode> _Nodes = new List<YSyntaxNode>();

        public void AddChild(YSyntaxNode node)
        {
            _Nodes.Add(node);
        }

        public IEnumerable<YSyntaxNode> Nodes { get { return _Nodes; } }
    }
}
