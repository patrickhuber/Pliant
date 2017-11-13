using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Grammars
{
    public class DottedRuleSet
    {
        private IDottedRule[] _cachedData;        
        private SortedSet<IDottedRule> _set;
        private Dictionary<ISymbol, DottedRuleSet> _reductions;
        private Dictionary<TokenType, DottedRuleSet> _tokenTransitions;
        private Dictionary<ILexerRule, DottedRuleSet> _scans;
        private List<ILexerRule> _scanKeys;

        public IReadOnlyList<IDottedRule> Data { get { return _cachedData; } }
                
        public IReadOnlyDictionary<ISymbol, DottedRuleSet> Reductions { get { return _reductions; } }

        public IReadOnlyDictionary<TokenType, DottedRuleSet> TokenTransitions { get { return _tokenTransitions; } }

        public IReadOnlyDictionary<ILexerRule, DottedRuleSet> Scans { get { return _scans; } }

        public IReadOnlyList<ILexerRule> ScanKeys { get { return _scanKeys; } }

        public DottedRuleSet NullTransition { get; set; }

        public DottedRuleSet(SortedSet<IDottedRule> set)
        {
            _set = set;
            _cachedData = _set.ToArray();
            _reductions = new Dictionary<ISymbol, DottedRuleSet>();
            _tokenTransitions = new Dictionary<TokenType, DottedRuleSet>();
            _scans = new Dictionary<ILexerRule, DottedRuleSet>();
            _scanKeys = new List<ILexerRule>();

            _hashCode = ComputeHashCode(set);
        }

        private readonly int _hashCode;

        public void AddTransistion(ISymbol symbol, DottedRuleSet target)
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

        public bool Contains(IDottedRule state)
        {
            return _set.Contains(state);
        }

        static int ComputeHashCode(SortedSet<IDottedRule> data)
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

            var dottedRuleSet = obj as DottedRuleSet;
            if (((object)dottedRuleSet) == null)
                return false;

            foreach (var item in _cachedData)
                if (!dottedRuleSet.Contains(item))
                    return false;

            return true;
        }
    }
}
