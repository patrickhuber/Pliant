using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class StringLiteralLexerRule : BaseLexerRule, IStringLiteralLexerRule
    {
        public static readonly LexerRuleType StringLiteralLexerRuleType = new LexerRuleType("StringLiteral");
        private readonly int _hashCode;

        public string Literal { get; private set; }

        public StringLiteralLexerRule(string literal, TokenType tokenType)
            : base(StringLiteralLexerRuleType, tokenType)
        {
            Literal = literal;
            _hashCode = ComputeHashCode(literal, StringLiteralLexerRuleType, tokenType);
        }

        public StringLiteralLexerRule(string literal)
            : this(literal, new TokenType(literal))
        { }

        public override string ToString()
        {
            return Literal;
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var terminalLexerRule = obj as StringLiteralLexerRule;
            if (((object)terminalLexerRule) == null)
                return false;
            return LexerRuleType.Equals(terminalLexerRule.LexerRuleType)
                && Literal.Equals(terminalLexerRule.Literal);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private static int ComputeHashCode(
            string literal,
            LexerRuleType terminalLexerRuleType,
            TokenType tokenType)
        {
            return HashCode.Compute(
                terminalLexerRuleType.GetHashCode(),
                tokenType.GetHashCode(),
                literal.GetHashCode());
        }

        public override bool CanApply(char c)
        {
            if (Literal.Length == 0)
                return false;
            return Literal[0].Equals(c);
        }
    }
}