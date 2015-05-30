using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Parser
    {
        private PulseRecognizer _pulseRecognizer;
        
        public int Location { get { return _pulseRecognizer.Location; } }

        public Parser(IGrammar grammar)
        {
            _pulseRecognizer = new PulseRecognizer(grammar);
        }

        public bool Pulse(char token)
        {
            return _pulseRecognizer.Pulse(token);
        }

        public bool Pulse(IToken token)
        {
            return _pulseRecognizer.Pulse(token);
        }

        public IEnumerable<ILexerRule> GetLexerRules()
        {
            var chart = _pulseRecognizer.Chart;
            var earleySets = chart.EarleySets;
            var currentIndex = earleySets.Count - 1;
            var currentEarleySet = earleySets[currentIndex];
            var scanStates = currentEarleySet.Scans;
            
            return from scanState in scanStates
                   let lexerRule = scanState.DottedRule.PostDotSymbol.Value as ILexerRule
                   group lexerRule by lexerRule.TokenType into rulesByTokenType
                   select rulesByTokenType.First();
        }

        public void Reset()
        {
            _pulseRecognizer.Reset();
        }

        public bool IsAccepted()
        {
            return _pulseRecognizer.IsAccepted();
        }

        public INode ParseForest()
        { 
            // thompson construction
            if (!IsAccepted())
                throw new Exception("Unable to parse expression.");
            var chart = _pulseRecognizer.Chart;
            var lastSet = chart.EarleySets[chart.Count - 1];
            var start = _pulseRecognizer.Grammar.Start;
            var completed = lastSet.Completions.First(x => x.Production.LeftHandSide.Equals(start));
            return completed.ParseNode;
        }        
    }
}
