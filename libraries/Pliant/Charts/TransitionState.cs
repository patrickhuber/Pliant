using Pliant.Grammars;

namespace Pliant.Charts
{
    public class TransitionState : State, ITransitionState
    {
        public ISymbol Recognized { get; private set; }
        
        public IState Reduction { get; private set; }
        
        public ITransitionState NextTransition { get; set; }

        public TransitionState(ISymbol recognized, IState transition, IState reduction)
            : base(transition.Production, transition.DottedRule.Position, transition.Origin)
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

        public override int GetHashCode()
        {
            return this.Recognized.GetHashCode() ^ base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Recognized, base.ToString());
        }

        public override StateType StateType { get { return StateType.Transitive; } }
    }
}
