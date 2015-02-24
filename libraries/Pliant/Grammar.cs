using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class Grammar : Earley.IGrammar
    {
        private ReadOnlyList<IProduction> _productions;
        public IReadOnlyList<IProduction> Productions { get { return _productions; } }

        public Grammar(params IProduction[] productions)
        {
            Assert.IsNotNullOrEmpty(productions, "productions");
            _productions = new ReadOnlyList<IProduction>(productions);
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal symbol)
        {
            foreach (var production in _productions)
            {
                var leftHandSide = production.LeftHandSide;
                if (leftHandSide.SymbolType == symbol.SymbolType
                    && leftHandSide.Value.Equals(symbol.Value))
                    yield return production;
            }
        }
    }
}
