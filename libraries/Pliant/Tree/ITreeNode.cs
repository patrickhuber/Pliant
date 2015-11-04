using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tree
{
    public interface ITreeNode
    {
        TreeNodeType NodeType { get; }
        int Origin { get; }
        int Location { get; }
        void Accept(ITreeNodeVisitor visitor);
    }
}
