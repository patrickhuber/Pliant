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

        public Grammar(INonTerminal start, IProduction[] productions, ILexeme[] lexemes, INonTerminal[] ignore)
        {
            Assert.IsNotNullOrEmpty(productions, "productions");
            Assert.IsNotNull(start, "start");
            Productions = new ReadOnlyList<IProduction>(productions);
            Lexemes = new ReadOnlyList<ILexeme>(lexemes);
            Ignore = new ReadOnlyList<INonTerminal>(ignore);
            Start = start; 
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


        public INonTerminal Start { get; private set; }

        public IReadOnlyList<INonTerminal> Ignore { get; private set; }
    }
}
