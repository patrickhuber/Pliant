using Pliant.Captures;
using Pliant.Grammars;

namespace Pliant.Tokens
{
    public class TerminalLexeme : LexemeBase<ITerminalLexerRule>, ILexeme
    {
        public ITerminal Terminal
        {
            get { return ConcreteLexerRule.Terminal; }
        }

        bool _isAccepted;
            
        public TerminalLexeme(ITerminalLexerRule lexerRule, ICapture<char> segment, int offset)
            : base(lexerRule, segment, offset)
        {         
            _isAccepted = false;
        }

        public TerminalLexeme(ITerminal terminal, TokenType tokenType, ICapture<char> segment, int offset)
            : this(new TerminalLexerRule(terminal, tokenType), segment, offset)
        {
        }

        public override void Reset()
        {
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

        public override bool Scan()
        {
            if (IsAccepted())
                return false;

            // TODO: verify that the segment is passed in at the correct position. it should be Count == 0 
            if (!Capture.Peek(out char value))
                return false;

            if (!Terminal.IsMatch(value))
                return false;

            if (!Capture.Grow())
                return false;

            SetAccepted(true);

            return true;
        }
    }
}