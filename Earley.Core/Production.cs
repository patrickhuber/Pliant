using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class Production : Earley.IProduction
    {
        public ISymbol LeftHandSide { get; private set; }

        private ReadOnlyList<ISymbol> _rightHandSide;

        public IReadOnlyList<ISymbol> RightHandSide { get { return _rightHandSide; } }

        public Production(ISymbol leftHandSide, params ISymbol[] rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, "leftHandSide");
            Assert.IsNotNullOrEmpty(rightHandSide, "rightHandSide");
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
            if (LeftHandSide != production.LeftHandSide)
                return false;
            if (RightHandSide.Count != production.RightHandSide.Count)
                return false;
            for (int i = 0; i < RightHandSide.Count; i++)
                if (RightHandSide[i] != production.RightHandSide[i])
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
    }
}
