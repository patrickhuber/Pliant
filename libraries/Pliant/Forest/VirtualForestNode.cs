using System;
using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Collections;
using Pliant.Charts;

namespace Pliant.Forest
{
    public class VirtualForestNode : ForestNodeBase, ISymbolForestNode
    {
        private ReadWriteList<IAndForestNode> _children;
        private List<VirtualForestNodePath> _paths;

        public IReadOnlyList<IAndForestNode> Children
        {
            get
            {
                if (!ResultCached())
                    LazyLoadChildren();
                return _children;
            }
        }
                
        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Symbol; }
        }

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
            _children = new ReadWriteList<IAndForestNode>();
            _paths = new List<VirtualForestNodePath>();

            var path = new VirtualForestNodePath(transitionState, completedParseNode);
            _paths.Add(path);

            Symbol = targetState.Production.LeftHandSide;
            if (IsUniqueChildSubTree(path))
                CloneUniqueChildSubTree(path.ForestNode as IInternalForestNode);
        }

        private bool IsUniqueChildSubTree(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;

            return transitionState.Reduction.ParseNode != null
            && completedParseNode == transitionState.Reduction.ParseNode
            && (completedParseNode.NodeType == ForestNodeType.Intermediate
                || completedParseNode.NodeType == ForestNodeType.Symbol);
        }

        private void CloneUniqueChildSubTree(IInternalForestNode internalCompletedParseNode)
        {
            for (var a = 0; a < internalCompletedParseNode.Children.Count; a++)
            {
                var andNode = internalCompletedParseNode.Children[a];
                var newAndNode = new AndForestNode();
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    newAndNode.AddChild(child);
                }
                _children.Add(newAndNode);
            }
        }
        
        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
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
        
        private static IState GetTargetState(ITransitionState transitionState)
        {
            var parameterTransitionStateHasNoParseNode = transitionState.ParseNode == null;
            if (parameterTransitionStateHasNoParseNode)
                return transitionState.Reduction;
            return transitionState;
        }

        private bool ResultCached()
        {
            return _children.Count > 0;
        }

        private void LazyLoadChildren()
        {
            for (int i = 0; i < _paths.Count; i++)
                LazyLoadPath(_paths[i]);
        }

        private void LazyLoadPath(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;
            if (transitionState.NextTransition != null)
            {
                var virtualNode = new VirtualForestNode(Location, transitionState.NextTransition, completedParseNode);

                if (transitionState.Reduction.ParseNode == null)
                    AddUniqueFamily(virtualNode);
                else
                    AddUniqueFamily(transitionState.Reduction.ParseNode, virtualNode);
            }
            else if (transitionState.Reduction.ParseNode != null)
            {
                AddUniqueFamily(transitionState.Reduction.ParseNode, completedParseNode);
            }
            else
            {
                AddUniqueFamily(completedParseNode);
            }
        }
    }
}
