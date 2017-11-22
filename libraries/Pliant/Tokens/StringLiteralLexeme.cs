using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tokens
{
    public class StringLiteralLexeme : LexemeBase<IStringLiteralLexerRule>, ILexeme
    {
        private string _capture;
        private int _index;
        
        public string Literal { get { return ConcreteLexerRule.Literal; } }

        public override string Value
        {
            get
            {
                if (!IsSubStringAllocated())
                    _capture = AllocateSubString();
                return _capture;
            }
        }        
        
        public StringLiteralLexeme(IStringLiteralLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            _index = 0;
            _capture = null;
        }

        private bool IsSubStringAllocated()
        {
            if (_capture == null)
                return false;
            return _index == _capture.Length;
        }

        private string AllocateSubString()
        {
            return Literal.Substring(0, _index);
        }

        public override bool IsAccepted()
        {
            return _index >= Literal.Length;
        }

        public override bool Scan(char c)
        {
            if (_index >= Literal.Length)
                return false;
            if (Literal[_index] != c)
                return false;
            _index++;
            return true;
        }

        public override void Reset()
        {
            _index = 0;
            _capture = null;
        }      
    }
}