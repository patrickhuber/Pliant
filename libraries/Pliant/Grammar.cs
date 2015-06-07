using Pliant.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pliant
{
    public class Grammar : IGrammar
    {
        public IReadOnlyList<IProduction> Productions { get; private set; }
        
        public IReadOnlyList<ILexerRule> LexerRules { get; private set; }

        public INonTerminal Start { get; private set; }

        public IReadOnlyList<ILexerRule> Ignores { get; private set; }

        private IDictionary<INonTerminal, IList<IProduction>> _productionIndex;

        public Grammar(INonTerminal start, IProduction[] productions, ILexerRule[] lexerRules, ILexerRule[] ignore)
        {
            Assert.IsNotNullOrEmpty(productions, "productions");
            Assert.IsNotNull(start, "start");
            CreateProductionIndex(productions);
            Productions = new ReadOnlyList<IProduction>(productions);
            LexerRules = new ReadOnlyList<ILexerRule>(lexerRules);
            Ignores = new ReadOnlyList<ILexerRule>(ignore);
            Start = start; 
        }

        private void CreateProductionIndex(IEnumerable<IProduction> productions)
        {
            _productionIndex = new Dictionary<INonTerminal, IList<IProduction>>();
            foreach (var production in productions)
            {
                var leftHandSide = production.LeftHandSide;
                if (!_productionIndex.ContainsKey(leftHandSide))
                    _productionIndex.Add(leftHandSide, new List<IProduction>());
                _productionIndex[leftHandSide].Add(production);
            }
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal symbol)
        {
            return _productionIndex[symbol];
        }

        public IEnumerable<ILexerRule> LexerRulesFor(INonTerminal symbol)
        {
            foreach (var lexerRule in LexerRules)
            {
                if (lexerRule.SymbolType == symbol.SymbolType
                    && lexerRule.TokenType.Id.Equals(symbol.Value))
                    yield return lexerRule;
            }
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return Productions.Where(p => p.LeftHandSide.Equals(Start));
        }
    }
}
