using Pliant.Forest;
using System;
using System.Collections;
using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Collections;
using Pliant.Tokens;

namespace Pliant.Tree
{
    public class ParseTreeEnumerator
        : IParseTreeEnumerator
    {
        IInternalForestNode _forestRoot;
        ParseTreeEnumeratorState _status;
        ForestNodeVisitorImpl _visitor;

        private enum ParseTreeEnumeratorState
        {
            New,
            Current,
            Done
        }

        public ParseTreeEnumerator(
            IInternalForestNode forestRoot)
        {
            _forestRoot = forestRoot;
            _status = ParseTreeEnumeratorState.New;
            _visitor = new ForestNodeVisitorImpl();
        }

        public ITreeNode Current
        {
            get
            {
                switch (_status)
                {
                    case ParseTreeEnumeratorState.Current:
                        return _visitor.Root;

                    default:
                        return null;
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            _visitor = null;
            _forestRoot = null;
        }

        public bool MoveNext()
        {
            if (_status == ParseTreeEnumeratorState.Done)
                return false;

            if (_forestRoot.NodeType == ForestNodeType.Intermediate)
                _visitor.Visit(_forestRoot as IIntermediateForestNode);
            else if (_forestRoot.NodeType == ForestNodeType.Symbol)
                _visitor.Visit(_forestRoot as ISymbolForestNode);
            
            if (_visitor.Root == null)
            {    _status = ParseTreeEnumeratorState.Done;
                return false;
            }

            _status = ParseTreeEnumeratorState.Current;
            return true;
        }

        public void Reset()
        {
            _status = ParseTreeEnumeratorState.New;
            _visitor.Reset();
        }

        private class ForestNodeVisitorImpl : ForestNodeVisitorBase
        {
            Dictionary<IInternalForestNode, int> _paths;
            HashSet<IInternalForestNode> _visited;

            Stack<InternalTreeNodeImpl> _nodeStack;
            IInternalForestNode _lock;
            int _count;

            public ITreeNode Root { get; private set; }

            public ForestNodeVisitorImpl()
            {
                _paths = new Dictionary<IInternalForestNode, int>();
                _nodeStack = new Stack<InternalTreeNodeImpl>();
                _visited = new HashSet<IInternalForestNode>();
                _count = 0;
            }

            public override void Visit(IIntermediateForestNode intermediateNode)
            {
                if (!_visited.Add(intermediateNode))
                    return;

                var childIndex = GetOrSetChildIndex(intermediateNode);
                var path = intermediateNode.Children[childIndex];
                
                Visit(path);
            }

            public override void Visit(ISymbolForestNode symbolNode)
            {
                if (!_visited.Add(symbolNode))
                    return;

                int childIndex = GetOrSetChildIndex(symbolNode);

                var path = symbolNode.Children[childIndex];
                var internalTreeNode = new InternalTreeNodeImpl(
                    symbolNode.Origin,
                    symbolNode.Location,
                    symbolNode.Symbol as INonTerminal);

                var isRoot = _nodeStack.Count == 0;
                if (!isRoot)
                {
                    _nodeStack
                        .Peek()
                        .ReadWriteChildren
                        .Add(internalTreeNode);
                }

                _nodeStack.Push(internalTreeNode);
                
                Visit(path);

                var top = _nodeStack.Pop();
                            
                if (isRoot)
                {
                    if (_count > 0 && _lock == null)
                        Root = null;
                    else
                        Root = top;
                    _count++;
                    _visited.Clear();
                }
            }

            private int GetOrSetChildIndex(IInternalForestNode symbolNode)
            {
                var childIndex = 0;
                if (!_paths.TryGetValue(symbolNode, out childIndex))
                {
                    _paths.Add(symbolNode, 0);
                    return childIndex;
                }

                var isLocked = !object.ReferenceEquals(null, _lock);
                if (!isLocked)
                    _lock = symbolNode;

                var isCurrentNodeLocked = object.ReferenceEquals(_lock, symbolNode);
                if(!isCurrentNodeLocked)
                    return childIndex;

                childIndex++;
                if (childIndex >= symbolNode.Children.Count)
                {
                    _lock = null;
                    _paths[symbolNode] = 0;
                    return 0;
                }
                _paths[symbolNode] = childIndex;
                return childIndex;
            }
            
            public override void Visit(ITerminalForestNode terminalNode)
            {
                var token = new Token(
                    terminalNode.Capture.ToString(), 
                    terminalNode.Origin, 
                    new TokenType(terminalNode.ToString()));
                VisitToken(terminalNode.Origin, terminalNode.Location, token);
            }
            
            public override void Visit(ITokenForestNode tokenNode)
            {
                VisitToken(tokenNode.Origin, tokenNode.Location, tokenNode.Token);
            }

            private void VisitToken(int origin, int location, IToken token)
            {
                var tokenTreeNodeImpl = new TokenTreeNodeImpl(
                    origin,
                    location,
                    token);

                var parent = _nodeStack.Peek();
                parent.ReadWriteChildren.Add(tokenTreeNodeImpl);
            }

            public void Reset()
            {
                _paths.Clear();
                _nodeStack.Clear();
                _visited.Clear();
                _count = 0;
                _lock = null;                
            }
        }

        private abstract class TreeNodeImpl : ITreeNode
        {
            public int Location { get; private set; }

            public TreeNodeType NodeType { get; private set; }

            public int Origin { get; private set; }

            protected TreeNodeImpl(int origin, int location, TreeNodeType nodeType)
            {
                Origin = origin;
                Location = location;
                NodeType = nodeType;
            }

            public abstract void Accept(ITreeNodeVisitor visitor);
        }

        private class InternalTreeNodeImpl : TreeNodeImpl, IInternalTreeNode
        {
            public List<ITreeNode> ReadWriteChildren { get; private set; }

            public IReadOnlyList<ITreeNode> Children
            {
                get { return ReadWriteChildren; }
            }

            public INonTerminal Symbol { get; private set; }

            public InternalTreeNodeImpl(int origin, int location, INonTerminal symbol)
                : base(origin, location, TreeNodeType.Internal)
            {
                Symbol = symbol;
                ReadWriteChildren = new List<ITreeNode>();
            }

            public override void Accept(ITreeNodeVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        private class TokenTreeNodeImpl : TreeNodeImpl, ITokenTreeNode
        {
            public TokenTreeNodeImpl(int origin, int location, IToken token) 
                : base(origin, location, TreeNodeType.Token)
            {
                Token = token;
            }

            public IToken Token { get; private set; }

            public override void Accept(ITreeNodeVisitor visitor)
            {
                visitor.Visit(this);
            }
        }        
    }
}
