using Pliant.Captures;
using Pliant.Grammars;

namespace Pliant.Tokens
{
    public class StringLiteralLexeme : LexemeBase<IStringLiteralLexerRule>, ILexeme
    {
        private int _index;
        
        public string Literal { get { return ConcreteLexerRule.Literal; } }

        public StringLiteralLexeme(IStringLiteralLexerRule lexerRule, ICapture<char> segment, int offset)
            : base(lexerRule, segment, offset)
        {
            _index = 0;
        }

        public override bool IsAccepted()
        {
            return _index >= Literal.Length;
        }

        public override bool Scan()
        {
            if (_index >= Literal.Length)
                return false;

            // TODO: verify that the segment is passed in at the correct position. it should be Count == 0 
            if (!Capture.Peek(out char value))
                return false;

            if (Literal[_index] != value)
                return false;
            
            _index++;

            return Capture.Grow();
        }

        public override void Reset()
        {
            _index = 0;
        }      
    }
}