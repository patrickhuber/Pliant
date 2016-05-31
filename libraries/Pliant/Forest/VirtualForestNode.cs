using Pliant.Charts;
using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public class VirtualForestNode : ForestNodeBase, ISymbolForestNode
    {
        private ITransitionState _transitionState;
        private IForestNode _completedParseNode;
        private ReadWriteList<IAndForestNode> _children;
        
        public ISymbol Symbol { get; private set; }

        public VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode)
            : this(
                  location,
                  transitionState, 
                  completedParseNode, 
                  GetTargetState(transitionState))
        {
        }

        protected VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode,
            IState targetState)
            : base(targetState.Origin, location)
        {
            _transitionState = transitionState;
            _completedParseNode = completedParseNode;
            _children = new ReadWriteList<IAndForestNode>();
            Symbol = targetState.Production.LeftHandSide;
            if (IsUniqueChildSubTree())
                CloneUniqueChildSubTree(_completedParseNode as IInternalForestNode);
        }

        private bool IsUniqueChildSubTree()
        {
            return _transitionState.Reduction.ParseNode != null
                && _completedParseNode == _transitionState.Reduction.ParseNode
                && (_completedParseNode.NodeType == ForestNodeType.Intermediate
                    || _completedParseNode.NodeType == ForestNodeType.Symbol);
        }

        private void CloneUniqueChildSubTree(IInternalForestNode internalCompletedParseNode)
        {
            foreach (var andNode in internalCompletedParseNode.Children)
            {
                var newAndNode = new AndForestNode();
                foreach (var child in andNode.Children)
                    newAndNode.AddChild(child);
                _children.Add(newAndNode);
            }
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Symbol; }
        }

        public IReadOnlyList<IAndForestNode> Children
        {
            get
            {
                if (!ResultCached())
                    LazyLoadChildren();
                return _children;
            }
        }

        private static IState GetTargetState(ITransitionState transitionState)
        {
            var parameterTransitionStateHasNoParseNode = transitionState.ParseNode == null;
            if (parameterTransitionStateHasNoParseNode)
                return transitionState.Reduction;
            return transitionState;
        }
        
        private void LazyLoadChildren()
        {
            if (_transitionState.NextTransition != null)
            {
                var virtualNode = new VirtualForestNode(Location, _transitionState.NextTransition, _completedParseNode);

                if (_transitionState.Reduction.ParseNode == null)
                    AddUniqueFamily(virtualNode);
                else
                    AddUniqueFamily(_transitionState.Reduction.ParseNode, virtualNode);
            }
            else if (_transitionState.Reduction.ParseNode != null)
            {
                AddUniqueFamily(_transitionState.Reduction.ParseNode, _completedParseNode);
            }
            else
            {
                AddUniqueFamily(_completedParseNode);
            }
        }

        private bool ResultCached()
        {
            return _children.Count != 0;
        }

        public void AddUniqueFamily(IForestNode trigger)
        {
            var andNode = new AndForestNode();
            andNode.AddChild(trigger);
            _children.Add(andNode);
        }

        public void AddUniqueFamily(IForestNode source, IForestNode trigger)
        {
            var andNode = new AndForestNode();
            andNode.AddChild(source);
            andNode.AddChild(trigger);
            _children.Add(andNode);
        }
        
        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}