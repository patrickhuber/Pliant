using System;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tokens
{
    public class TerminalLexeme : ILexeme
    {
        public ITerminal Terminal { get; private set; }

        string _stringCapture;
        char _capture;
        bool _captureRendered;
        bool _isAccepted;

        public int Position { get; private set; }

        public string Value
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

        public TokenType TokenType { get { return LexerRule.TokenType; } }

        public ILexerRule LexerRule { get; private set; }

        public TerminalLexeme(ITerminalLexerRule lexerRule, int position)
        {
            Reset(lexerRule, position);
        }

        public TerminalLexeme(ITerminal terminal, TokenType tokenType, int position)
            : this(new TerminalLexerRule(terminal, tokenType), position)
        {
        }

        public void Reset(ITerminalLexerRule terminalLexerRule, int position)
        {
            LexerRule = terminalLexerRule;
            Terminal = terminalLexerRule.Terminal;
            _captureRendered = false;
            _isAccepted = false;
            Position = position;
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

        public bool Scan(ILexContext context, char c)
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