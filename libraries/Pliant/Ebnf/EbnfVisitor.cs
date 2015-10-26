using Pliant.Ast;
using Pliant.Grammars;
using Pliant.Builders;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : NodeVisitorBase
    {
        public IGrammar Grammar { get { return _grammarbuilder.ToGrammar(); } }

        private GrammarBuilder _grammarbuilder;

        public EbnfVisitor()
            : base()
        {
        }

        public EbnfVisitor(INodeVisitorStateManager stateManager)
            : base(stateManager)
        {
        }
                        
        public override void Visit(ISymbolNode node)
        {
            switch (node.Symbol.SymbolType)
            {
                case SymbolType.NonTerminal:
                    var leftHandSide = (node.Symbol as INonTerminal);
                    switch (leftHandSide.Value)
                    {
                        case "Grammar":
                            VisitGrammar(node);
                            break;

                        case "Rule":
                            VisitRule(node);
                            break;

                        case "Setting":
                            VisitSetting(node);
                            break;

                        default:
                            BeginVisitInternalNodeChildren(node);
                            EndVisitInternalNodeChildren(node);
                            break;
                    }
                    break;
            }
        }

        private void VisitGrammar(ISymbolNode node)
        {
            _grammarbuilder = new GrammarBuilder();
            BeginVisitInternalNodeChildren(node);
            EndVisitInternalNodeChildren(node);
        }

        private void VisitRule(ISymbolNode node)
        {
            ;
        }

        private void VisitSetting(ISymbolNode node)
        {

        }

        public override void Visit(IIntermediateNode node)
        {
            // intermediate nodes are just used to binarize the tree, 
            // we don't actually process them in any way
            BeginVisitInternalNodeChildren(node);
            EndVisitInternalNodeChildren(node);
        }

        protected virtual void VisitInternalNodeChildren(IIntermediateNode node)
        {

        }

        protected void BeginVisitInternalNodeChildren(IInternalNode node)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(node);
            foreach (var child in currentAndNode.Children)
                child.Accept(this);
        }

        protected virtual void EndVisitInternalNodeChildren(IInternalNode node)
        {
            StateManager.MarkAsTraversed(node);
        }


        public override void Visit(ITokenNode tokenNode)
        {
            ;
        }

        public override void Visit(ITerminalNode node)
        {
            ;
        }

        public override void Visit(IAndNode andNode)
        {
            ;
        }
    }
}
