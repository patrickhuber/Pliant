using System;
using Pliant.Forest;
using System.Collections.Generic;
using System.IO;

namespace Pliant.Tests.Common.Forest
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
            _visited.Add(tokenNode);
            return;
        }

        public void Visit(IAndForestNode andNode)
        {
            for (var i = 0; i < andNode.Children.Count; i++)
            {
                var child = andNode.Children[i];
                child.Accept(this);
            }
        }

        public void Visit(IIntermediateForestNode node)
        {
            if (!_visited.Add(node))
                return;

            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                Visit(child);
            }
        }

        public void Visit(ISymbolForestNode node)
        {
            if (!_visited.Add(node))
                return;
            
            for (var a = 0; a < node.Children.Count; a++)
            {
                PrintNode(node);
                _writer.Write(" ->");
                var andNode = node.Children[a];
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    PrintNode(child);
                }
                _writer.WriteLine();
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                Visit(child);
            }
        }

        private void PrintNode(IForestNode node)
        {
            switch (node.NodeType)
            {
                case ForestNodeType.Intermediate:
                    var intermediate = node as IIntermediateForestNode;
                    if (intermediate.Children.Count > 1)
                        throw new Exception("Intermediate node has more children than expected. ");
                    var flatList = GetFlattenedList(intermediate);
                    for (var i = 0; i < flatList.Count; i++)
                    {
                        _writer.Write(" ");
                        PrintNode(flatList[i]);
                    }
                    break;

                case ForestNodeType.Symbol:
                    var symbolForestNode = node as ISymbolForestNode;
                    var symbolForestNodeString = GetSymbolNodeString(symbolForestNode);
                    _writer.Write(" ");
                    _writer.Write(symbolForestNodeString);
                    break;
                    
                case ForestNodeType.Token:
                    var tokenForestNode = node as ITokenForestNode;
                    var tokenForestNodeString = GetTokenNodeString(tokenForestNode);
                    _writer.Write(" ");
                    _writer.Write(tokenForestNodeString);
                    break;
            }
        }
    

        private static IList<IForestNode> GetFlattenedList(IIntermediateForestNode intermediate)
        {
            var children = new List<IForestNode>();
            for (var a = 0; a < intermediate.Children.Count; a++)
            {
                var andNode = intermediate.Children[a];
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    switch (child.NodeType)
                    {
                        case ForestNodeType.Intermediate:
                            var childList = GetFlattenedList(child as IIntermediateForestNode);
                            children.AddRange(childList);
                            break;
                        default:
                            children.Add(child);
                            break;
                    }
                }
            }
            return children;
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
            return $"('{node.Token.Value}', {node.Origin}, {node.Location})";
        }
        
        public void Visit(ITerminalForestNode node)
        {
            throw new NotImplementedException();
        }
    }
}
