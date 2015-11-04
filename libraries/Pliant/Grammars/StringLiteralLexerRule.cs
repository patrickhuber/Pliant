using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class StringLiteralLexerRule : BaseLexerRule, IStringLiteralLexerRule
    {
        public static readonly LexerRuleType StringLiteralLexerRuleType = new LexerRuleType("StringLiteral");

        public string Literal { get; private set; }
        
        
        public StringLiteralLexerRule(string literal, TokenType tokenType)
            : base(StringLiteralLexerRuleType, tokenType)
        {
            Literal = literal;
        }

        public StringLiteralLexerRule(string literal)
            : this(literal, new TokenType(literal))
        { }

        public override string ToString()
        {
            return Literal;
        }
    }
}
