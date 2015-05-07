using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Lexeme 
    {
        private ILexerRule _lexerRule;
        private PulseRecognizer _pulseRecognizer;

        public Lexeme(ILexerRule lexerRule)
        {
            _lexerRule = lexerRule;
        }

        public bool Pulse(char character)
        {
            return _pulseRecognizer.Pulse(character);
        }
    }
}
