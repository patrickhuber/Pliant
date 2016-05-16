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

        /// <summary>
        /// A single AND node. Virtual nodes are leo nodes and by nature don't have ambiguity.
        /// </summary>
        private AndForestNode _andNode;

        public ISymbol Symbol { get; private set; }

        public VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode)
            : base(GetTargetState(transitionState).Origin, location)
        {
            _transitionState = transitionState;
            _completedParseNode = completedParseNode;
            _children = new ReadWriteList<IAndForestNode>();
            Symbol = GetTargetState(transitionState).Production.LeftHandSide;
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
            return _andNode != null;
        }

        public void AddUniqueFamily(IForestNode trigger)
        {
            if (_andNode != null)
                return;
            _andNode = new AndForestNode();
            _andNode.AddChild(trigger);
            _children.Add(_andNode);
        }

        public void AddUniqueFamily(IForestNode source, IForestNode trigger)
        {
            if (_andNode != null)
                return;
            _andNode = new AndForestNode();
            _andNode.AddChild(source);
            _andNode.AddChild(trigger);
            _children.Add(_andNode);
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