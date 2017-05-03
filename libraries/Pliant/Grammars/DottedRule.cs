using Pliant.Utilities;
using System.Text;
using System;
using Pliant.Diagnostics;

namespace Pliant.Grammars
{
    public class DottedRule : IComparable<DottedRule>, IDottedRule
    {
        private readonly int _hashCode;
        
        public IProduction Production { get; private set; }

        public int Position { get; private set; }
                
        public DottedRule(IProduction production, int position)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));

            Production = production;
            Position = position;
            _hashCode = ComputeHashCode(Production, Position);
        }
        
        private static int ComputeHashCode(IProduction production, int position)
        {
            return HashCode.Compute(production.GetHashCode(), position.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var preComputedState = obj as DottedRule;
            if (((object)preComputedState) == null)
                return false;
            return preComputedState.Production.Equals(Production)
                && preComputedState.Position == Position;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", Production.LeftHandSide.Value);
            const string Dot = "\u25CF";

            for (var p = 0; p < Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == Position ? Dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Position == Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            return stringBuilder.ToString();
        }

        public int CompareTo(DottedRule other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(IDottedRule other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }
    }
}
