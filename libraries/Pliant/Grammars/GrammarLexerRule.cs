using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class GrammarLexerRule : IGrammarLexerRule
    {
        public IGrammar Grammar { get; private set; }

        public TokenType TokenType { get; private set; }

        public LexerRuleType LexerRuleType { get { return LexerRuleType.Grammar; } }

        public GrammarLexerRule(string tokenType, IGrammar grammar)
            : this(new TokenType(tokenType), grammar)
        {
        }

        public GrammarLexerRule(TokenType tokenType, IGrammar grammar)
        {
            Grammar = grammar;
            TokenType = tokenType;
        }
        
        public SymbolType SymbolType
        {
            get { return SymbolType.LexerRule; }
        }

        public override string ToString()
        {
            return TokenType.Id;
        }
    }
}
