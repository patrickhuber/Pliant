using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Lexemes
{
    public class TerminalLexeme : ILexeme
    {
        public ITerminal Terminal { get; private set; }

        string _stringCapture;
        char _capture;
        bool _captureRendered;
        bool _isAccepted;

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
            _captureRendered = false;
            _isAccepted = false;
        }

        public bool IsAccepted()
        {
            return _isAccepted;
        }

        void SetAccepted(bool value)
        {
            _isAccepted = value;
        }

        void SetCapture(char value)
        {
            _capture = value;
        }

        public bool Scan(char c)
        {
            if (IsAccepted())
                return false;

            if (!Terminal.IsMatch(c))
                return false;
            
            SetCapture(c);
            SetAccepted(true);
            return true;
        }
    }
}