using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class BaseLexerRule : ILexerRule
    {
        protected LexerRuleType _lexerRuleType;
        protected TokenType _tokenType;

        protected BaseLexerRule(LexerRuleType lexerRuleType, TokenType tokenType)
        {
            _lexerRuleType = lexerRuleType;
            _tokenType = tokenType;
        }

        public LexerRuleType LexerRuleType { get { return _lexerRuleType; } }

        public SymbolType SymbolType
        {
            get { return SymbolType.LexerRule; }
        }

        public TokenType TokenType
        {
            get { return _tokenType; }
        }

        public abstract bool CanApply(char c);
    }
}