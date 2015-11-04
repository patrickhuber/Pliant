using Pliant.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Grammars
{
    public class Production : IProduction
    {
        public INonTerminal LeftHandSide { get; private set; }

        private ReadWriteList<ISymbol> _rightHandSide;

        public IReadOnlyList<ISymbol> RightHandSide { get { return _rightHandSide; } }

        public bool IsEmpty { get { return _rightHandSide.Count == 0; } }

        public Production(INonTerminal leftHandSide, params ISymbol[] rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, "leftHandSide");
            Assert.IsNotNull(rightHandSide, "rightHandSide");
            LeftHandSide = leftHandSide;
            _rightHandSide = new ReadWriteList<ISymbol>(new List<ISymbol>(rightHandSide));
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

        // PERF: Cache Costly Hash Code Computation
        private bool _isHashCodeComputed = false;
        private int _computedHashCode = 0;

        public override int GetHashCode()
        {
            if (_isHashCodeComputed)
                return _computedHashCode;
            unchecked
            {
                var hash = (int)2166136261;
                hash *= 16777619 * LeftHandSide.GetHashCode();
                foreach (var symbol in RightHandSide)
                    hash *= 16777619 ^ symbol.GetHashCode();

                _computedHashCode = hash;
                _isHashCodeComputed = true;
                return _computedHashCode;
            }
        }
        
        public void AddSymbol(ISymbol symbol)
        {
            InvalidateCachedHashCode();
            _rightHandSide.Add(symbol);
        }

        private void InvalidateCachedHashCode()
        {
            _isHashCodeComputed = false;
        }

        public Production Clone()
        {
            var production = new Production(LeftHandSide);
            foreach(var symbol in RightHandSide)
                production.AddSymbol(symbol);
            return production;
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
