using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class TransitionState : State
    {
        public ISymbol Recognized { get; private set; }

        public TransitionState(ISymbol recognized, IProduction production, int position, int start)
            : base(production, position, start)
        {
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

        public override StateType StateType { get { return Earley.StateType.Transitive; } }
    }
}
