using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Lexer
    {
        private PulseRecognizer _recognizer;
        
        public Lexer(IGrammar grammar)
        {
            _recognizer = new PulseRecognizer(grammar);
        }

        public IEnumerable<IToken> Scan(char c)
        {
            _recognizer.Pulse(c);
            return DiscoverTokens(_recognizer.Chart);
        }

        private IEnumerable<IToken> DiscoverTokens(Chart chart)
        {

            yield break;
        }
    }
}
