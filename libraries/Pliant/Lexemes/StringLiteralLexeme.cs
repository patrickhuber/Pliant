using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;
using System.Text;

namespace Pliant.Lexemes
{
    public class StringLiteralLexeme : ILexeme
    {
        private StringBuilder _stringBuilder;
        private string _capture;

        private int _index = 0;

        public string Literal { get; private set; }

        public string Capture
        {
            get
            {
                if (IsStringBuilderAllocated())
                    DeallocateStringBuilderAndAssignCapture();
                return _capture;
            }
        }

        private bool IsStringBuilderAllocated()
        {
            return _stringBuilder != null;
        }

        private void DeallocateStringBuilderAndAssignCapture()
        {
            SharedPools.Default<StringBuilder>().Free(_stringBuilder);
            _capture = _stringBuilder.ToString();
            _stringBuilder = null;
        }

        private void ReallocateStringBuilderFromCapture()
        {
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            _stringBuilder.Append(_capture);
        }

        public TokenType TokenType { get; private set; }

        public StringLiteralLexeme(string literal, TokenType tokenType)
        {
            Literal = literal;
            TokenType = tokenType;
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
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
            if (!IsStringBuilderAllocated())
                ReallocateStringBuilderFromCapture();
            _stringBuilder.Append(c);
            return true;
        }
    }
}