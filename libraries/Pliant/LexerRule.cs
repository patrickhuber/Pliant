using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class LexerRule : ILexerRule
    {
        public IGrammar Grammar { get; private set; }

        public LexerRule(INonTerminal identifier,
            ITerminal[] terminals)
            : this(identifier, terminals, Repetition.One)
        { }

        public LexerRule(INonTerminal identifier,
            ITerminal[] terminals,
            Repetition repetition)
        {
            switch (repetition)
            {
                case Repetition.OneOrMany:
                    Grammar = OneOrMany(identifier, terminals);
                    break;
                case Repetition.ZeroOrMany:
                    Grammar = ZeroOrMany(identifier, terminals);
                    break;
                case Repetition.ZeroOrOne:
                    Grammar = ZeroOrOne(identifier, terminals);
                    break;
                default:
                    Grammar = Once(identifier, terminals);
                    break;
            }        
        }


        private static IGrammar OneOrMany(INonTerminal identifier, ITerminal[] terminals)
        {
            var recursiveProductionSymbols = new List<ISymbol>();
            recursiveProductionSymbols.Add(identifier);
            recursiveProductionSymbols.AddRange(terminals);
            
            // NonTerminal -> NonTerminal Terminal | Terminal                    
            return new Grammar(
                identifier,
                new[]{
                    new Production(identifier, recursiveProductionSymbols.ToArray()),
                    new Production(identifier, terminals)},
                new IProduction[] { },
                new INonTerminal[] { });
        }

        private static IGrammar ZeroOrMany(INonTerminal identifier, ITerminal[] terminals)
        {
            var recursiveProductionSymbols = new List<ISymbol>();
            recursiveProductionSymbols.Add(identifier);
            recursiveProductionSymbols.AddRange(terminals);
         
            // NonTerminal -> NonTerminal Terminal | <null>
            return new Grammar(
                identifier,
                new[]{
                    new Production(identifier, recursiveProductionSymbols.ToArray()),
                    new Production(identifier)},
                new IProduction[] { },
                new INonTerminal[] { });
        }

        private static IGrammar ZeroOrOne(INonTerminal identifier, ITerminal[] terminals)
        {
            // NonTerminal -> Terminal | <null>
            return new Grammar(
                identifier,
                new[]{ 
                    new Production(identifier, terminals), 
                    new Production(identifier) },
                new IProduction[] { },
                new INonTerminal[] { });
        }

        private static IGrammar Once(INonTerminal identifier, ITerminal[] terminals)
        {
            // NonTerminal -> Terminal
            return new Grammar(
                identifier, new[]{ 
                    new Production(identifier, terminals)},
                new IProduction[] { },
                new INonTerminal[] { });
        }
    }
}
