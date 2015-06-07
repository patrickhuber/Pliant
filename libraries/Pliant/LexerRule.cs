using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant
{
    public class LexerRule : ILexerRule
    {
        public IGrammar Grammar { get; private set; }

        public TokenType TokenType { get; private set; }

        public LexerRule(INonTerminal leftHandSide, IGrammar grammar)
        {
            Grammar = grammar;
            TokenType = new TokenType(leftHandSide.Value);
        }

        private LexerRule(INonTerminal leftHandSide, string regularExpression)
        {
            var regexGrammar = new RegexGrammar();
        }

        public SymbolType SymbolType
        {
            get { return SymbolType.LexerRule; }
        }

    }
}
