﻿using Pliant.Forest;
using Pliant.Grammars;
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
            if (obj is null)
                return false;
            if (!(obj is NormalState state))
                return false;
            // PERF: Hash Codes are Cached, so equality performance is cached as well
            return GetHashCode() == state.GetHashCode();
        }

        private int ComputeHashCode()
        {
            return NormalStateHashCodeAlgorithm.Compute(
                DottedRule,
                Origin);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "ToString is not called in performance critical code")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0502:Explicit new reference type allocation", Justification = "ToString is not called in performance critical code")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in performance critical code")]
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