using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Grammars
{
    public class Frame
    {
        private PreComputedState[] _cachedData;        
        private SortedSet<PreComputedState> _set;
        private Dictionary<ISymbol, Frame> _reductions;
        private Dictionary<TokenType, Frame> _tokenTransitions;
        private Dictionary<ILexerRule, Frame> _scans;
        private List<ILexerRule> _scanKeys;

        public IReadOnlyList<PreComputedState> Data { get { return _cachedData; } }
                
        public IReadOnlyDictionary<ISymbol, Frame> Reductions { get { return _reductions; } }

        public IReadOnlyDictionary<TokenType, Frame> TokenTransitions { get { return _tokenTransitions; } }

        public IReadOnlyDictionary<ILexerRule, Frame> Scans { get { return _scans; } }

        public IReadOnlyList<ILexerRule> ScanKeys { get { return _scanKeys; } }

        public Frame NullTransition { get; set; }

        public Frame(SortedSet<PreComputedState> set)
        {
            _set = set;
            _cachedData = _set.ToArray();
            _reductions = new Dictionary<ISymbol, Frame>();
            _tokenTransitions = new Dictionary<TokenType, Frame>();
            _scans = new Dictionary<ILexerRule, Frame>();
            _scanKeys = new List<ILexerRule>();

            _hashCode = ComputeHashCode(set);
        }

        private readonly int _hashCode;

        public void AddTransistion(ISymbol symbol, Frame target)
        {
            if (symbol.SymbolType == SymbolType.NonTerminal)
            {
                if (!Reductions.ContainsKey(symbol))
                    _reductions.Add(symbol, target);
            }
            else if(symbol.SymbolType == SymbolType.LexerRule)
            {
                var lexerRule = symbol as ILexerRule;
                if (!Scans.ContainsKey(lexerRule))
                {
                    _tokenTransitions.Add(lexerRule.TokenType, target);
                    _scans.Add(lexerRule, target);
                    _scanKeys.Add(lexerRule);
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
