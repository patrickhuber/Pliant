using Pliant.Forest;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Common
{    
    public class StatefulForestNodeComparer : IEqualityComparer<IForestNode>
    {
        HashSet<IForestNode> _traversed;

        public StatefulForestNodeComparer()
        {
            _traversed = new HashSet<IForestNode>();
        }

        public bool Equals(IForestNode firstForestNode, IForestNode secondForestNode)
        {
            if (!_traversed.Add(firstForestNode))
                return true;

            if (firstForestNode.NodeType != secondForestNode.NodeType)
                return false;

            switch (firstForestNode.NodeType)
            {
                case ForestNodeType.Intermediate:
                    return AreIntermediateNodesEqual(
                        firstForestNode as IIntermediateForestNode,
                        secondForestNode as IIntermediateForestNode);

                case ForestNodeType.Symbol:
                    return AreSymbolNodesEqual(
                        firstForestNode as ISymbolForestNode,
                        secondForestNode as ISymbolForestNode);   
                                     
                case ForestNodeType.Terminal:
                    return AreTerminalNodesEqual(
                        firstForestNode as ITerminalForestNode,
                        secondForestNode as ITerminalForestNode);

                case ForestNodeType.Token:
                    return AreTokenNodesEqual(
                        firstForestNode as ITokenForestNode,
                        secondForestNode as ITokenForestNode);
                default:
                    return false;
            }
        }

        bool AreChildNodesEqual(IInternalForestNode firstInternalForestNode, IInternalForestNode secondInternalForestNode)
        {
            if (firstInternalForestNode.Children.Count != secondInternalForestNode.Children.Count)
                return false;

            for (int i = 0; i < firstInternalForestNode.Children.Count; i++)
            {
                if (!AreAndNodesEqual(
                    firstInternalForestNode.Children[i],
                    secondInternalForestNode.Children[i]))
                    return false;
            }
            return true;
        }

        bool AreAndNodesEqual(IAndForestNode firstAndNode, IAndForestNode secondAndNode)
        {
            if (firstAndNode.Children.Count != secondAndNode.Children.Count)
                return false;

            for (int i = 0; i < firstAndNode.Children.Count; i++)
            {
                if (!Equals(
                    firstAndNode.Children[i],
                    secondAndNode.Children[i]))
                    return false;
            }
            return true;
        }

        bool AreIntermediateNodesEqual(IIntermediateForestNode firstIntermediateForestNode, IIntermediateForestNode secondIntermediateForestNode)
        {
            if (!firstIntermediateForestNode.DottedRule.Equals(
                secondIntermediateForestNode.DottedRule))
                return false;
            return AreChildNodesEqual(firstIntermediateForestNode, secondIntermediateForestNode);
        }

        bool AreSymbolNodesEqual(ISymbolForestNode firstSymbolForestNode, ISymbolForestNode secondSymbolForestNode)
        {
            if (!firstSymbolForestNode.Symbol.Equals(secondSymbolForestNode.Symbol))
                return false;
            return AreChildNodesEqual(firstSymbolForestNode, secondSymbolForestNode);
        }

        static bool AreTerminalNodesEqual(ITerminalForestNode firstTerminalForestNode, ITerminalForestNode secondTerminalForestNode)
        {
            return firstTerminalForestNode.Capture == secondTerminalForestNode.Capture;
        }

        static bool AreTokenNodesEqual(ITokenForestNode firstTokenForestNode, ITokenForestNode secondForestTokenNode)
        {
            return firstTokenForestNode.Token.TokenType.Id ==
                secondForestTokenNode.Token.TokenType.Id
                && firstTokenForestNode.Token.Value ==
                secondForestTokenNode.Token.Value;
        }

        public int GetHashCode(IForestNode obj)
        {
            return obj.GetHashCode();
        }
    }
}
