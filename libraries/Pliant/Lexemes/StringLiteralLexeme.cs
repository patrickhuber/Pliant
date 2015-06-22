using System.Text;
using Pliant.Tokens;
using Pliant.Grammars;

namespace Pliant.Lexemes
{
    public class StringLiteralLexeme : ILexeme
    {
        private StringBuilder _capture;
        
        private int _index = 0;

        public string Literal { get; private set; }

        public string Capture
        {
            get { return _capture.ToString(); }
        }

        public TokenType TokenType { get; private set; }

        public StringLiteralLexeme(string literal, TokenType tokenType)
        {
            Literal = literal;
            TokenType = tokenType;
            _capture = new StringBuilder();
        }

        public StringLiteralLexeme(IStringLiteralLexerRule lexerRule)
            : this(lexerRule.Literal, lexerRule.TokenType)
        { }

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
            _capture.Append(c);
            return true;
        }
    }
}
