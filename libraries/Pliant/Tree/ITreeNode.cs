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
        void Accept(ITreeNodeVisitor visitor);
    }
}
