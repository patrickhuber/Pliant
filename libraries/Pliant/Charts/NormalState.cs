using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;
using System.Text;

namespace Pliant.Charts
{
    public class NormalState : StateBase, INormalState
    {        
        private readonly int _hashCode;

        public NormalState(IDottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
            _hashCode = ComputeHashCode();
        }

        public NormalState(IDottedRule dottedRule, int origin, IForestNode parseNode) 
            : this(dottedRule, origin)
        {
            ParseNode = parseNode;
        }

        public override StateType StateType { get { return StateType.Normal; } }

        public bool IsSource(ISymbol searchSymbol)
        {
            var dottedRule = DottedRule;
            if (dottedRule.IsComplete)
                return false;
            return dottedRule.PostDotSymbol.Equals(searchSymbol);
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
                DottedRule.Position.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.Production.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", DottedRule.Production.LeftHandSide.Value);
            const string Dot = "\u25CF";

            for (int p = 0; p < DottedRule.Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == DottedRule.Position ? Dot : " ",
                    DottedRule.Production.RightHandSide[p]);
            }

            if (DottedRule.Position == DottedRule.Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            stringBuilder.Append($"\t\t({Origin})");
            return stringBuilder.ToString();
        }        
    }
}