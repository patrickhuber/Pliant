using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Lexemes
{
    public class Lexeme : ILexeme
    {
        public string Capture { get; private set; }

        public TokenType TokenType { get; private set; }

        private IParseEngine _parseEngine;
        private PulseRecognizer _pulseRecognizer;

        public Lexeme(TokenType tokenType, IParseEngine lexicalParseEngine)
        {
            TokenType = tokenType;
            _parseEngine = lexicalParseEngine;
        }

        public Lexeme(ILexerRule lexerRule)
        {
            Capture = string.Empty;
            TokenType = lexerRule.TokenType;
            _pulseRecognizer = new PulseRecognizer(lexerRule.Grammar);
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
