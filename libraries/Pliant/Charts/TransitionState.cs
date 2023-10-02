using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{    
    public class TransitionState : StateBase, ITransitionState
    {
        public ISymbol Symbol { get; private set; }

        public ITransitionState NextTransition { get; set; }

        public int Root { get; set; }
                        
        public TransitionState(
            ISymbol recognized,
            IDottedRule dottedRule,
            int origin)
            : base(dottedRule, origin)
        {
            Symbol = recognized;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is not TransitionState transitionState)
                return false;

            return GetHashCode() == transitionState.GetHashCode()
                && Symbol.Equals(transitionState.Symbol);
        }
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.GetHashCode(),
                Origin.GetHashCode(),
                Symbol.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"{Symbol} : {DottedRule}\t\t({Origin})";
        }

        public IDynamicForestNodePath Next()
        {
            return NextTransition;
        }

        public IForestNode Node()
        {
            return ParseNode;
        }

        public override StateType StateType { get { return StateType.Transitive; } }
    }
}