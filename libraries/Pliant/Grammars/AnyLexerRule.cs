using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Grammars
{
    public class AnyLexerRule : BaseLexerRule
    {
        public static readonly LexerRuleType AnyLexerRuleType = new LexerRuleType("Any");

        public AnyLexerRule(string tokenType)
            : this(new TokenType(tokenType))
        { 
        }

        public AnyLexerRule(TokenType tokenType)
            : base(AnyLexerRuleType, tokenType)
        { }

        public override bool CanApply(char c)
        {
            return true;
        }

        public override string ToString()
        {
            return TokenType.Id;
        }
    }
}
