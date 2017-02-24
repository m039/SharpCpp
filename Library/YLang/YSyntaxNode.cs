using System;
using System.Collections.Generic;

namespace SharpCpp
{
    public interface YSyntaxNode
    {
        void AddChild(YSyntaxNode node);

        IEnumerable<YSyntaxNode> Nodes { get; }
    }
}
