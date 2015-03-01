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
        
        public IReadOnlyList<ILexeme> Lexemes { get; private set; }

        public Grammar(IProduction[] productions, ILexeme[] lexemes)
        {
            Assert.IsNotNullOrEmpty(productions, "productions");
            Productions = new ReadOnlyList<IProduction>(productions);
            Lexemes = new ReadOnlyList<ILexeme>(lexemes);
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal symbol)
        {
            foreach (var production in Productions)
            {
                var leftHandSide = production.LeftHandSide;
                if (leftHandSide.SymbolType == symbol.SymbolType
                    && leftHandSide.Value.Equals(symbol.Value))
                    yield return production;
            }
        }
    }
}
