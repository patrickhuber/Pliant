using Pliant.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.RegularExpressions
{
    public class RegexVisitor : TreeNodeVisitorBase
    {
        public Regex Regex { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            base.Visit(node);
        }

        public override void Visit(ITokenTreeNode node)
        {
            base.Visit(node);
        }
    }
}
