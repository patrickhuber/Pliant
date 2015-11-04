using Pliant.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tree
{
    public abstract class TreeNodeVisitorBase : ITreeNodeVisitor
    {
        public TreeNodeVisitorBase() 
            : base() { }
        
        public virtual void Visit(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
                child.Accept(this);
        }

        public virtual void Visit(ITokenTreeNode node)
        {
        }        
    }
}
