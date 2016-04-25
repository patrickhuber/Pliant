using Pliant.Grammars;
using System;

namespace Pliant.Charts
{
    public class TransitionState : State, ITransitionState
    {
        public ISymbol Recognized { get; private set; }

        public IState Reduction { get; private set; }

        public int Index { get; private set; }

        public ITransitionState NextTransition { get; set; }

        public TransitionState(
            ISymbol recognized,
            IState transition,
            IState reduction,
            int index)
            : base(transition.Production, transition.Position, transition.Origin)
        {
            Reduction = reduction;
            Recognized = recognized;
            Index = index;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            var transitionState = obj as TransitionState;
            if (transitionState == null)
                return false;
            return base.Equals(obj as State) && this.Recognized.Equals(transitionState.Recognized);
        }
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                this.Recognized.GetHashCode(),
                base.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"{Recognized} : {base.ToString()}";
        }

        public override StateType StateType { get { return StateType.Transitive; } }
    }
}