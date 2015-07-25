using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class StringLiteralLexerRule : IStringLiteralLexerRule
    {
        public static readonly LexerRuleType StringLiteralLexerRuleType = new LexerRuleType("StringLiteral");

        public string Literal { get; private set; }

        public LexerRuleType LexerRuleType
        {
            get { return StringLiteralLexerRuleType; }
        }

        public SymbolType SymbolType { get { return SymbolType.LexerRule; } }

        public TokenType TokenType { get; private set; }

        public StringLiteralLexerRule(string literal, TokenType tokenType)
        {
            Literal = literal;
            TokenType = tokenType;
        }

        public override string ToString()
        {
            return Literal;
        }
    }
}
