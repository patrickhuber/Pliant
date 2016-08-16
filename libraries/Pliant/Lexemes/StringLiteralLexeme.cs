using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Lexemes
{
    public class StringLiteralLexeme : ILexeme
    {
        private string _capture;
        private int _index;

        public string Literal { get; private set; }

        public string Capture
        {
            get
            {
                if (!IsSubStringAllocated())
                    _capture = AllocateSubString();
                return _capture;
            }
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

        public TokenType TokenType { get; private set; }

        public StringLiteralLexeme(string literal, TokenType tokenType)
        {
            Reset(literal, tokenType);
        }

        public StringLiteralLexeme(IStringLiteralLexerRule lexerRule)
        {
            Reset(lexerRule);
        }

        public bool IsAccepted()
        {
            return _index >= Literal.Length;
        }

        public bool Scan(char c)
        {
            if (_index >= Literal.Length)
                return false;
            if (Literal[_index] != c)
                return false;
            _index++;
            return true;
        }

        public void Reset(IStringLiteralLexerRule newLiteral)
        {
            Reset(newLiteral.Literal, newLiteral.TokenType);
        }

        public void Reset(string literal, TokenType tokenType)
        {
            _index = 0;
            _capture = null;
            Literal = literal;
            TokenType = tokenType;
        }
    }
}