using System;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexerRule : IDfaLexerRule
    {
        public static readonly LexerRuleType DfaLexerRuleType = new LexerRuleType("Dfa");
        public LexerRuleType LexerRuleType { get { return DfaLexerRuleType; } }

        public IDfaState Start { get; private set; }

        public SymbolType SymbolType
        {
            get { return SymbolType.LexerRule; }
        }

        public TokenType TokenType { get; private set; }

        public DfaLexerRule(IDfaState state, TokenType tokenType)
        {
            Start = state;
            TokenType = tokenType;
        }
    }
}
