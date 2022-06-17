using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class TransitionState : StateBase, ITransitionState
    {
        public ISymbol Recognized { get; private set; }

        public IState Reduction { get; set; }

        public ITransitionState NextTransition { get; set; }
                
        public TransitionState(
            ISymbol recognized,
            IState reduction,
            int origin)
            : base(reduction.DottedRule, origin)
        {
            Reduction = reduction;
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
            return $"{Recognized} : {Reduction.DottedRule}\t\t({Origin})";
        }

        public override StateType StateType { get { return StateType.Transitive; } }

        public IState GetTargetState()
        {
            var parameterTransitionStateHasNoParseNode = ParseNode is null;
            if (parameterTransitionStateHasNoParseNode)
                return Reduction;
            return this;
        }
    }
}