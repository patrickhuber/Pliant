using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public class NodeWalker
    {
        public void Walk(INode node)
        {
            switch (node.NodeType)
            {
                case NodeType.Intermediate:
                    WalkIntermediate(node as IIntermediateNode);
                    break;
                case NodeType.Symbol:
                    WalkSymbol(node as ISymbolNode);
                    break;
                case NodeType.Terminal:
                    WalkTerminal(node as ITerminalNode);
                    break;
                case NodeType.Token:
                    WalkToken(node as ITokenNode);
                    break;
                case NodeType.Virtual:
                    WalkInternal(node as ISymbolNode);
                    break;
            }
        }

        private void WalkIntermediate(IIntermediateNode intermediateNode)
        {
            Debug.WriteLine(intermediateNode.State.Production.LeftHandSide);
            var firstAndNode = intermediateNode.Children[0];
            foreach (var node in firstAndNode.Children)
                Walk(node);
        }

        private void WalkSymbol(ISymbolNode symbolNode)
        {
            Debug.WriteLine(symbolNode.Symbol);
            var firstAndNode = symbolNode.Children[0];
            foreach (var node in firstAndNode.Children)
                Walk(node);
        }

        private void WalkTerminal(ITerminalNode terminalNode)
        {
            bool test = true;
        }

        private void WalkToken(ITokenNode tokenNode)
        {
            bool test = true;
        }
        
        private void WalkInternal(ISymbolNode internalNode)
        {
            var firstAndNode = internalNode.Children[0];
            foreach (var node in firstAndNode.Children)
                Walk(node);
        }
    }
}
