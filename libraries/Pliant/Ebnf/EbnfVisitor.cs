using Pliant.Grammars;
using Pliant.Tree;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public EbnfDefinition Definition { get; private set; }
        
        public override void Visit(IInternalTreeNode node)
        {
            
        }
    }
}