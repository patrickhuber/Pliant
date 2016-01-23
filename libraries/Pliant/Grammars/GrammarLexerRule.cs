using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class GrammarLexerRule : BaseLexerRule, IGrammarLexerRule
    {
        public IGrammar Grammar { get; private set; }

        public static readonly LexerRuleType GrammarLexerRuleType = new LexerRuleType("Grammar");

        public GrammarLexerRule(string tokenType, IGrammar grammar)
            : this(new TokenType(tokenType), grammar)
        {
        }

        public GrammarLexerRule(TokenType tokenType, IGrammar grammar)
            : base(GrammarLexerRuleType, tokenType)
        {
            Grammar = grammar;
        }

        public override string ToString()
        {
            return TokenType.Id;
        }
    }
}