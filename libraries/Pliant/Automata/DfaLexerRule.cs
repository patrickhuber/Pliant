using System;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class DfaLexerRule : BaseLexerRule, IDfaLexerRule
    {
        public static readonly LexerRuleType DfaLexerRuleType = new LexerRuleType("Dfa");
        private readonly int _hashCode;

        public IDfaState Start { get; private set; }

        public DfaLexerRule(IDfaState state, string tokenType)
            : this(state, new TokenType(tokenType))
        {
        }

        public DfaLexerRule(IDfaState state, TokenType tokenType)
            : base(DfaLexerRuleType, tokenType)
        {
            Start = state;
            _hashCode = ComputeHashCode(DfaLexerRuleType, tokenType);
        }

        private static int ComputeHashCode(LexerRuleType dfaLexerRuleType, TokenType tokenType)
        {
            return HashCode.Compute(
                dfaLexerRuleType.GetHashCode(),
                tokenType.GetHashCode());
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var dfaLexerRule = obj as DfaLexerRule;
            if (((object)dfaLexerRule) == null)
                return false;
            return TokenType.Equals(dfaLexerRule.TokenType);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool CanApply(char c)
        {
            for (var i = 0; i < Start.Transitions.Count; i++)
            {
                var transition = Start.Transitions[i];
                if (transition.Terminal.IsMatch(c))
                    return true;
            }
            return false;
        }
    }
}