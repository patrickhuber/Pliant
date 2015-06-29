using Pliant.Grammars;

namespace Pliant.Charts
{
    public class TransitionState : State, ITransitionState
    {
        public ISymbol Recognized { get; private set; }
        
        public IState Reduction { get; private set; }
        
        public ITransitionState NextTransition { get; set; }

        public TransitionState(ISymbol recognized, IState transition, IState reduction)
            : base(transition.Production, transition.Position, transition.Origin)
        {
            Reduction = reduction;
            Recognized = recognized;
        }
        
        public override bool Equals(object obj)
        {
            var transitionState = obj as TransitionState;
            if (transitionState == null)
                return false;
            return base.Equals(obj as State) && this.Recognized.Equals(transitionState.Recognized);
        }

        private int _computedHashCode = 0;
        private bool _isHashCodeComputed = false;

        public override int GetHashCode()
        {
            if (_isHashCodeComputed)
                return _computedHashCode;
            _computedHashCode = HashUtil.ComputeHash(
                this.Recognized.GetHashCode(),
                base.GetHashCode());
            _isHashCodeComputed = true;
            return _computedHashCode;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Recognized, base.ToString());
        }

        public override StateType StateType { get { return StateType.Transitive; } }
    }
}
