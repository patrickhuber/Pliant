using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class DynamicForestNodeLinkAdapter : IDynamicForestNodeLink
    {
        readonly ITransitionState _transitionState;
        IDynamicForestNodeLink _next;
        IDynamicForestNodeLink _first;

        public IForestNode Bottom => _transitionState?.Bottom?.ParseNode;

        public IForestNode Top => _transitionState?.Top?.ParseNode;

        public IDynamicForestNodeLink First { get; private set; }

        public IDynamicForestNodeLink Next
        {
            get
            {
                if (!(_next is null))
                    return _next;

                if (_transitionState?.Next is null)
                    return null;

                var next = new DynamicForestNodeLinkAdapter(_transitionState.Next);
                if (!(_first is null))
                    next._first = _first;
                _next = next;
                return _next;
            }
        }

        public ISymbol Symbol => _transitionState.Recognized;

        public DynamicForestNodeLinkAdapter(ITransitionState transitionState)
        { 
            _transitionState = transitionState;

            // is this the first transition state in the chain? if so, set the link to the first node
            if (ReferenceEquals(_transitionState.First, _transitionState))
                _first = this;
        }
    }
    
    public class TransitionState : StateBase, ITransitionState
    {
        public ISymbol Recognized { get; private set; }

        public IState Top { get; private set; }

        public IState Bottom { get; private set; }

        public ITransitionState Next { get; set; }

        /// <summary>
        /// First provides a way to jump to the root of the reduction path very easily
        /// </summary>
        public ITransitionState First { get; set; }
                
        public TransitionState(
            ISymbol recognized,
            IState top,
            IState bottom,
            int origin)
            : base(top.DottedRule, origin)
        {
            Top = top;
            Bottom = bottom;

            Recognized = recognized;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is TransitionState transitionState))
                return false;

            return GetHashCode() == transitionState.GetHashCode()
                && Recognized.Equals(transitionState.Recognized);
        }
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.GetHashCode(),
                Origin.GetHashCode(),
                Recognized.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"{Recognized} : {Top.DottedRule}\t\t({Origin})";
        }

        public override StateType StateType { get { return StateType.Transitive; } }

        public IState GetTargetState()
        {
            var parameterTransitionStateHasNoParseNode = ParseNode is null;
            if (parameterTransitionStateHasNoParseNode)
                return Top;
            return this;
        }
    }
}