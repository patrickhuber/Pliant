using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Lexeme : ILexeme
    {
        public ILexerRule LexerRule { get; private set; }

        public string Capture { get; private set; }

        private PulseRecognizer _pulseRecognizer;

        public Lexeme(ILexerRule lexerRule)
        {
            LexerRule = lexerRule;
            Capture = string.Empty;
            _pulseRecognizer = new PulseRecognizer(LexerRule.Grammar);
        }
        
        public bool Scan(char c)
        {
            var result = _pulseRecognizer.Pulse(c);
            if (result)
                Capture += c;
            return result;
        }
        
        public bool IsAccepted()
        {
            return _pulseRecognizer.IsAccepted();
        }
    }
}
