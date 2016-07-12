using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Lexemes
{
    public class TerminalLexeme : ILexeme
    {
        public ITerminal Terminal { get; private set; }

        private string _stringCapture;
        private char _capture;
        private bool _captureRendered = false;
        private bool _isAccepted = false;
                
        public string Capture
        {
            get
            {
                if (!_isAccepted)
                    return string.Empty;

                if (_captureRendered)
                    return _stringCapture;
                
                _stringCapture = _capture.ToString();
                _captureRendered = true;

                return _stringCapture;
            }
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
        }

        public bool IsAccepted()
        {
            return _isAccepted;
        }

        private void SetAccepted(bool value)
        {
            _isAccepted = value;
        }

        public bool Scan(char c)
        {
            if (!IsAccepted())
            {
                if (Terminal.IsMatch(c))
                {
                    _capture = c;
                    SetAccepted(true);
                    return true;
                }
            }
            return false;
        }
    }
}