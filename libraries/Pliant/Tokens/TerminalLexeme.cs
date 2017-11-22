using System;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tokens
{
    public class TerminalLexeme : LexemeBase<ITerminalLexerRule>, ILexeme
    {
        public ITerminal Terminal
        {
            get { return ConcreteLexerRule.Terminal; }
        }

        string _stringCapture;
        char _capture;
        bool _captureRendered;
        bool _isAccepted;
                
        public override string Value
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

        public TerminalLexeme(ITerminalLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            _captureRendered = false;
            _isAccepted = false;
        }

        public TerminalLexeme(ITerminal terminal, TokenType tokenType, int position)
            : this(new TerminalLexerRule(terminal, tokenType), position)
        {
        }

        public override void Reset()
        {
            _captureRendered = false;
            _isAccepted = false;
        }
                
        public override bool IsAccepted()
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

        public override bool Scan(char c)
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