using System;
using Pliant.Forest;
using System.Collections.Generic;
using System.IO;

namespace Pliant.Forest
{
    public class LoggingForestNodeVisitor : IForestNodeVisitor
    {
        private HashSet<IForestNode> _visited;
        private TextWriter _writer;

        public LoggingForestNodeVisitor(TextWriter streamWriter)
        {
            _visited = new HashSet<IForestNode>();
            _writer = streamWriter;
        }

        public void Visit(ITokenForestNode tokenNode)
        {
        }

        /// <summary>
        /// Visits each option 
        /// </summary>
        /// <param name="packedNode"></param>
        public void Visit(IPackedForestNode node)
        {
            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                PrintNode(child);
            }
        }

        public void Visit(IIntermediateForestNode node)
        {
            if (!_visited.Add(node))
                return;
            PrintNode(node);
            for (var i = 0; i < node.Children.Count; i++)
            {
                if (i > 0)
                {
                    _writer.WriteLine();
                    _writer.Write("\t|");
                }
                Visit(node.Children[i]);
            }
            _writer.WriteLine();
        }

        public void Visit(ISymbolForestNode node)
        {
            if (!_visited.Add(node))
                return;
            PrintNode(node);
            _writer.Write(" -> ");
            for (var i = 0; i < node.Children.Count; i++)
            {
                if (i > 0)
                {
                    _writer.WriteLine();
                    _writer.Write("\t|");                    
                }                
                Visit(node.Children[i]);
            }
            _writer.WriteLine();
        }

        public void Visit(ITerminalForestNode node)
        {
        }

        private void PrintNode(IForestNode node)
        {
            switch (node.NodeType)
            {
                case ForestNodeType.Symbol:
                    PrintNode(node as ISymbolForestNode);
                    break;
                case ForestNodeType.Intermediate:
                    PrintNode(node as IIntermediateForestNode);
                    break;
                case ForestNodeType.Terminal:
                    PrintNode(node as ITerminalForestNode);
                    break;
                case ForestNodeType.Token:
                    PrintNode(node as ITokenForestNode);
                    break;
            }
        }

        private void PrintNode(IIntermediateForestNode node)
        {
            var intermediateForestNodeString = GetIntermediateNodeString(node);
            _writer.Write(" ");
            _writer.Write(intermediateForestNodeString);
        }

        private void PrintNode(ISymbolForestNode node)
        {
            var symbolForestNodeString = GetSymbolNodeString(node);
            _writer.Write(" ");
            _writer.Write(symbolForestNodeString);
        }

        private void PrintNode(ITokenForestNode node)
        {
            var tokenForestNodeString = GetTokenNodeString(node);
            _writer.Write(" ");
            _writer.Write(tokenForestNodeString);
        }

        private void PrintNode(ITerminalForestNode node)
        {
            var terminalNodeString = GetTerminalNodeString(node);
            _writer.Write(" ");
            _writer.Write(terminalNodeString);
        }

        private static string GetSymbolNodeString(ISymbolForestNode node)
        {
            return $"({node.Symbol}, {node.Origin}, {node.Location})";
        }

        private static string GetIntermediateNodeString(IIntermediateForestNode node)
        {
            return $"({node.DottedRule}, {node.Origin}, {node.Location})";
        }

        private static string GetTokenNodeString(ITokenForestNode node)
        {
            return $"('{node.Token.Capture}', {node.Origin}, {node.Location})";
        }

        private static string GetTerminalNodeString(ITerminalForestNode node)
        {
            return $"('{node.Capture}', {node.Origin}, {node.Location})";
        }
        
    }
}
