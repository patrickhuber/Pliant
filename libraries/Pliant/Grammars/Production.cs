using Pliant.Collections;
using Pliant.Diagnostics;
using Pliant.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Grammars
{
    public class Production : IProduction
    {
        public INonTerminal LeftHandSide { get; private set; }

        private List<ISymbol> _rightHandSide;

        public IReadOnlyList<ISymbol> RightHandSide { get { return _rightHandSide; } }

        public bool IsEmpty { get { return _rightHandSide.Count == 0; } }
        
        public Production(INonTerminal leftHandSide, List<ISymbol> rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, nameof(leftHandSide));
            Assert.IsNotNull(rightHandSide, nameof(rightHandSide));
            LeftHandSide = leftHandSide;
            _rightHandSide = new List<ISymbol>(new List<ISymbol>(rightHandSide));
            _hashCode = ComputeHashCode();
        }

        public Production(INonTerminal leftHandSide, params ISymbol[] rightHandSide)
        {
            Assert.IsNotNull(leftHandSide, nameof(leftHandSide));
            Assert.IsNotNull(rightHandSide, nameof(rightHandSide));
            LeftHandSide = leftHandSide;
            _rightHandSide = new List<ISymbol>(new List<ISymbol>(rightHandSide));
            _hashCode = ComputeHashCode();
        }
                
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var production = obj as Production;
            if (production == null)
                return false;
            if (!LeftHandSide.Equals(production.LeftHandSide))
                return false;
            var rightHandSideCount = RightHandSide.Count;
            if (rightHandSideCount != production.RightHandSide.Count)
                return false;
            for (int i = 0; i < rightHandSideCount; i++)
                if (!RightHandSide[i].Equals(production.RightHandSide[i]))
                    return false;
            return true;
        }

        // PERF: Cache Costly Hash Code Computation
        private readonly int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private int ComputeHashCode()
        {
            var hash = HashCode.ComputeIncrementalHash(LeftHandSide.GetHashCode(), 0, true);
            
            for (var s = 0; s < RightHandSide.Count; s++)
            {
                var symbol = RightHandSide[s];
                hash = HashCode.ComputeIncrementalHash(symbol.GetHashCode(), hash);
            }
            return hash;
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