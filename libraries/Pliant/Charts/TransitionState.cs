using Pliant.Grammars;
using Pliant.Utilities;
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
            if ((object)obj == null)
                return false;
            var transitionState = obj as TransitionState;
            if ((object)transitionState == null)
                return false;
            return base.Equals(obj as State) 
                && this.Recognized.Equals(transitionState.Recognized)
                && this.Index == transitionState.Index;
        }
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.ComputeHash(
                Position.GetHashCode(),
                Origin.GetHashCode(),
                Production.GetHashCode(),
                Recognized.GetHashCode(),
                Reduction.GetHashCode(),
                Index.GetHashCode());
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