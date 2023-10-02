using System.Collections.Generic;

namespace Pliant.Forest
{
    public class PackedForestNode : IPackedForestNode
    {
        public IReadOnlyList<IForestNode> Children { get { return _children; } }

        private readonly List<IForestNode> _children;

        public PackedForestNode()
        {
            _children = new List<IForestNode>();
        }

        public void AddChild(IForestNode orNode)
        {
            _children.Add(orNode);
        }

        public void Accept(IForestNodeVisitor visitor)
        {
            for (var i = 0; i < _children.Count; i++)
            { 
                var child = _children[i];
                child.Accept(visitor);
            }
        }
    }
}