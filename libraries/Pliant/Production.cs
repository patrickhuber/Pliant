using Pliant.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pliant
{
    public class Production : IProduction
    {
        public INonTerminal LeftHandSide { get; private set; }

        private ReadOnlyList<ISymbol> _rightHandSide;

        public IReadOnlyList<ISymbol> RightHandSide { get { return _rightHandSide; } }

        public bool IsEmpty { get { return _rightHandSide.Count == 0; } }

        public Production(INonTerminal leftHandSide, params ISymbol[] rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, "leftHandSide");
            Assert.IsNotNull(rightHandSide, "rightHandSide");
            LeftHandSide = leftHandSide;
            _rightHandSide = new ReadOnlyList<ISymbol>(rightHandSide);
        }

        public Production(string leftHandSide, params ISymbol[] rightHandSide)
            : this(new NonTerminal(leftHandSide), rightHandSide)
        { 
        }

        public override bool Equals(object obj)
        {
            var production = obj as Production;
            if (production == null)
                return false;
            if (!LeftHandSide.Equals(production.LeftHandSide))
                return false;
            if (RightHandSide.Count != production.RightHandSide.Count)
                return false;
            for (int i = 0; i < RightHandSide.Count; i++)
                if (!RightHandSide[i].Equals(production.RightHandSide[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = LeftHandSide.GetHashCode();
            foreach (var symbol in RightHandSide)
                hashCode ^= symbol.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} ->", LeftHandSide.Value);
            for (int p = 0; p < RightHandSide.Count; p++)
            {
                var symbol = RightHandSide[p];
                stringBuilder.AppendFormat(" {0}", symbol);
            }
            return stringBuilder.ToString();
        }
    }
}
