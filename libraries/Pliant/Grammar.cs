using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Grammar : IGrammar
    {
        public IReadOnlyList<IProduction> Productions { get; private set; }
        
        public IReadOnlyList<IProduction> Lexemes { get; private set; }

        private IDictionary<INonTerminal, IList<IProduction>> _productionIndex;

        public Grammar(INonTerminal start, IProduction[] productions, IProduction[] lexemes, INonTerminal[] ignore)
        {
            Assert.IsNotNullOrEmpty(productions, "productions");
            Assert.IsNotNull(start, "start");
            CreateProductionIndex(productions);
            Productions = new ReadOnlyList<IProduction>(productions);
            Lexemes = new ReadOnlyList<IProduction>(lexemes);
            Ignores = new ReadOnlyList<INonTerminal>(ignore);
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

        public IEnumerable<IProduction> LexemesFor(INonTerminal symbol)
        {
            foreach (var lexeme in Lexemes)
            {
                var leftHandSide = lexeme.LeftHandSide;
                if (leftHandSide.SymbolType == symbol.SymbolType
                    && leftHandSide.Value.Equals(symbol.Value))
                    yield return lexeme;
            }
        }

        public INonTerminal Start { get; private set; }

        public IReadOnlyList<INonTerminal> Ignores { get; private set; }

        public IEnumerable<IProduction> StartProductions()
        {
            return Productions.Where(p => p.LeftHandSide.Equals(Start));
        }
    }
}
