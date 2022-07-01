using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class DynamicForestNodeLinkAdapter : IDynamicForestNodeLink
    {
        readonly ITransitionState _transitionState;
        IDynamicForestNodeLink _next;

        public IForestNode Bottom => _transitionState?.Bottom?.ParseNode;

        public IDynamicForestNodeLink First { get; private set; }

        public IDynamicForestNodeLink Next
        {
            get
            {
                if (_next is not null)
                    return _next;

                if (_transitionState?.Next is null)
                    return null;

                var next = new DynamicForestNodeLinkAdapter(_transitionState.Next);
                if (First is not null)
                    next.First = First;
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
                First = this;
        }
    }
    
    public class TransitionState : StateBase, ITransitionState
    {
        public ISymbol Recognized { get; private set; }

        public IState Bottom { get; private set; }

        public ITransitionState Next { get; set; }

        /// <summary>
        /// First provides a way to jump to the root of the reduction path very easily
        /// </summary>
        public ITransitionState First { get; set; }
                
        public TransitionState(
            ISymbol recognized,
            IDottedRule dottedRule,
            IState bottom,
            int origin)
            : base(dottedRule, origin)
        {
            Bottom = bottom;

            Recognized = recognized;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is not TransitionState transitionState)
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
            return $"{Recognized} : {DottedRule}\t\t({Origin})";
        }

        public override StateType StateType { get { return StateType.Transitive; } }
    }
}