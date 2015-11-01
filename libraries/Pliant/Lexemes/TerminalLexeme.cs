using Pliant.Grammars;
using Pliant.Tokens;
using System.Text;

namespace Pliant.Lexemes
{
    public class TerminalLexeme : ILexeme
    {
        public ITerminal Terminal { get; private set; }

        private StringBuilder _captureBuilder;

        public string Capture
        {
            get { return _captureBuilder.ToString(); }
        }

        public TokenType TokenType { get; private set; }

        public TerminalLexeme(ITerminalLexerRule lexerRule)
            : this(lexerRule.Terminal, lexerRule.TokenType)
        {
        }

        public TerminalLexeme(ITerminal terminal, TokenType tokenType)
        {
            Terminal = terminal;
            TokenType = tokenType;
            _captureBuilder = new StringBuilder();
        }

        public bool IsAccepted()
        {
            return _captureBuilder.Length > 0;
        }

        public bool Scan(char c)
        {
            if (!IsAccepted())
            {
                if (Terminal.IsMatch(c))
                {
                    _captureBuilder.Append(c);
                    return true;
                }
            }
            return false;
        }
    }
}
