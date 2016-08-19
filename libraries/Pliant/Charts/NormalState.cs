using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Charts
{
    public class NormalState : StateBase, INormalState
    {        
        private readonly int _hashCode;
        public NormalState(IProduction production, int position, int origin)
            : base(production, position, origin)
        {
            _hashCode = ComputeHashCode();
        }
        
        public IState NextState()
        {
            if (IsComplete)
                return null;
            var state = new NormalState(
                Production,
                Position + 1,
                Origin);
            return state;
        }
        
        public override StateType StateType { get { return StateType.Normal; } }

        public bool IsSource(ISymbol searchSymbol)
        {
            if (IsComplete)
                return false;
            return PostDotSymbol.Equals(searchSymbol);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var state = obj as NormalState;
            if (state == null)
                return false;
            // PERF: Hash Codes are Cached, so equality performance is cached as well
            return GetHashCode() == state.GetHashCode();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Position.GetHashCode(),
                Origin.GetHashCode(),
                Production.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", Production.LeftHandSide.Value);
            const string Dot = "\u25CF";

            for (int p = 0; p < Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == Position ? Dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Position == Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            stringBuilder.Append($"\t\t({Origin})");
            return stringBuilder.ToString();
        }        
    }
}