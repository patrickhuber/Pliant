using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    /// <summary>
    /// A Lexeme is something special. It is not a production.
    /// </summary>
    public class Lexeme
    {
        private StringBuilder _catpure;
        private PulseRecognizer _recognizer;

        public Lexeme(INonTerminal identifier, ITerminal terminal)
            : this(identifier, terminal, Repetition.One)
        {
        }

        public Lexeme(INonTerminal identifier, ITerminal terminal, Repetition repetition)
        {
            IGrammar grammar = null;
            switch (repetition)
            {
                case Repetition.OneOrMany:
                    grammar = OneOrMany(identifier, terminal);
                    break;
                case Repetition.ZeroOrMany:
                    grammar = ZeroOrMany(identifier, terminal);
                    break;
                case Repetition.ZeroOrOne:
                    grammar = ZeroOrOne(identifier, terminal);
                    break;
                default:
                    grammar = Once(identifier, terminal);
                    break;
            }
            _catpure = new StringBuilder();
            _recognizer = new PulseRecognizer(grammar);
        }

        private static IGrammar OneOrMany(INonTerminal identifier, ITerminal terminal)
        {
            // NonTerminal -> NonTerminal Terminal | Terminal                    
            return new Grammar(
                identifier,
                new[]{
                    new Production(identifier, identifier, terminal),
                    new Production(identifier, terminal)},
                new IProduction[] { },
                new INonTerminal[]{});
        }

        private static IGrammar ZeroOrMany(INonTerminal identifier, ITerminal terminal)
        {
            // NonTerminal -> NonTerminal Terminal | <null>
            return new Grammar(
                identifier,
                new[]{
                    new Production(identifier, identifier, terminal),
                    new Production(identifier)},
                new IProduction[] { },
                new INonTerminal[]{});
        }

        private static IGrammar ZeroOrOne(INonTerminal identifier, ITerminal terminal)
        {
            // NonTerminal -> Terminal | <null>
            return new Grammar(
                identifier,
                new[]{ 
                    new Production(identifier, terminal), 
                    new Production(identifier) },
                new IProduction[]{},
                new INonTerminal[]{});
        }

        private static IGrammar Once(INonTerminal identifier, ITerminal terminal)
        { 
            // NonTerminal -> Terminal
            return new Grammar(
                identifier, new []{ 
                    new Production(identifier, terminal)} ,
                new IProduction[] { },
                new INonTerminal[]{});
        }
        
        public string Capture { get { return _catpure.ToString(); } }
        
        public bool Match(char c)
        {
            int originalChartSize = _recognizer.Chart.Count;
            _recognizer.Pulse(c);
            bool characterWasMatched =  originalChartSize < _recognizer.Chart.Count;
            if (characterWasMatched)
                _catpure.Append(c);
            return characterWasMatched;
        }

    }
}
