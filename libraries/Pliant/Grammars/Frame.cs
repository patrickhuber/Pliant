using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Grammars
{
    internal class Frame
    {
        private PreComputedState[] _cachedData;        
        private SortedSet<PreComputedState> _set;

        public PreComputedState[] Data { get { return _cachedData; } }
                
        public Dictionary<ISymbol, Frame> Transitions { get; private set; }
        public Dictionary<TokenType, Frame> TokenTransitions { get; private set; }
        public Dictionary<ILexerRule, Frame> Scans { get; private set; }

        public Frame NullTransition { get; set; }

        public Frame(SortedSet<PreComputedState> set)
        {
            _set = set;
            _cachedData = _set.ToArray();
            Transitions = new Dictionary<ISymbol, Frame>();
            TokenTransitions = new Dictionary<TokenType, Frame>();
            Scans = new Dictionary<ILexerRule, Frame>();
            _hashCode = ComputeHashCode(set);
        }

        private readonly int _hashCode;

        public void AddTransistion(ISymbol symbol, Frame target)
        {
            Frame value = null;
            if (!Transitions.TryGetValue(symbol, out value))
            {
                Transitions.Add(symbol, target);
                if (symbol.SymbolType == SymbolType.LexerRule)
                {
                    var lexerRule = symbol as ILexerRule;
                    TokenTransitions.Add(lexerRule.TokenType, target);
                    Scans.Add(lexerRule, target);
                }
            }
        }

        public bool Contains(PreComputedState state)
        {
            return _set.Contains(state);
        }

        static int ComputeHashCode(SortedSet<PreComputedState> data)
        {
            return HashCode.Compute(data);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;

            var frame = obj as Frame;
            if (((object)frame) == null)
                return false;

            foreach (var item in _cachedData)
                if (!frame.Contains(item))
                    return false;

            return true;
        }
    }
}
