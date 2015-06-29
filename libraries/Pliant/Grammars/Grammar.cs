using Pliant.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        public IReadOnlyList<IProduction> Productions { get; private set; }
        
        public IReadOnlyList<ILexerRule> LexerRules { get; private set; }

        public INonTerminal Start { get; private set; }

        public IReadOnlyList<ILexerRule> Ignores { get; private set; }

        private IDictionary<INonTerminal, IList<IProduction>> _productionIndex;
        private IDictionary<int, IList<ILexerRule>> _lexerRuleIndex;
        
        private static readonly IProduction[] EmptyProductionArray = new IProduction[] { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = new ILexerRule[] { };

        public Grammar(INonTerminal start, IProduction[] productions, ILexerRule[] lexerRules, ILexerRule[] ignore)
        {
            Assert.IsNotNullOrEmpty(productions, "productions");
            Assert.IsNotNull(start, "start");
            _productionIndex = CreateProductionIndex(productions);
            _lexerRuleIndex = CreateLexerRuleIndex(lexerRules);
            Productions = new ReadOnlyList<IProduction>(productions ?? EmptyProductionArray);
            LexerRules = new ReadOnlyList<ILexerRule>(lexerRules ?? EmptyLexerRuleArray);
            Ignores = new ReadOnlyList<ILexerRule>(ignore ?? EmptyLexerRuleArray);
            Start = start; 
        }

        private IDictionary<INonTerminal, IList<IProduction>> CreateProductionIndex(IEnumerable<IProduction> productions)
        {
            var dictionary = new Dictionary<INonTerminal, IList<IProduction>>();
            foreach (var production in productions)
            {
                var leftHandSide = production.LeftHandSide;
                if (!dictionary.ContainsKey(leftHandSide))
                    dictionary.Add(leftHandSide, new List<IProduction>());
                dictionary[leftHandSide].Add(production);
            }
            return dictionary;
        }

        private IDictionary<int, IList<ILexerRule>> CreateLexerRuleIndex(IEnumerable<ILexerRule> lexerRules)
        {
            var dictionary = new Dictionary<int, IList<ILexerRule>>();
            foreach (var lexerRule in lexerRules)
            {
                var key = HashUtil.ComputeHash(
                    lexerRule.SymbolType.GetHashCode(),
                    lexerRule.TokenType.Id.GetHashCode());
                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, new List<ILexerRule>());
                dictionary[key].Add(lexerRule); 
            }
            return dictionary;
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal symbol)
        {
            IList<IProduction> list;
            if (!_productionIndex.TryGetValue(symbol, out list))
                return EmptyProductionArray;
            return list;
        }

        public IEnumerable<ILexerRule> LexerRulesFor(INonTerminal symbol)
        {
            var key = HashUtil.ComputeHash(
                symbol.SymbolType.GetHashCode(),
                symbol.Value.GetHashCode());
            IList<ILexerRule> list;
            if (!_lexerRuleIndex.TryGetValue(key, out list))
                return EmptyLexerRuleArray;
            return list;
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return RulesFor(Start);
        }
    }
}
