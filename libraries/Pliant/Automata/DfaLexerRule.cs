using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexerRule : BaseLexerRule, IDfaLexerRule
    {
        public static readonly LexerRuleType DfaLexerRuleType = new LexerRuleType("Dfa");

        public IDfaState Start { get; private set; }

        public DfaLexerRule(IDfaState state, string tokenType)
            : this(state, new TokenType(tokenType))
        {
        }

        public DfaLexerRule(IDfaState state, TokenType tokenType)
            : base(DfaLexerRuleType, tokenType)
        {
            Start = state;
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }
    }
}