using Pliant.Grammars;
using Pliant.Lexemes;
using System;

namespace Pliant.Automata
{
    public class DfaLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return DfaLexerRule.DfaLexerRuleType; } }

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    string.Format(
                        "Unable to create DfaLexeme from type {0}. Expected DfaLexerRule",
                        lexerRule.GetType().FullName));
            var dfaLexerRule = lexerRule as IDfaLexerRule;
            return new DfaLexeme(dfaLexerRule.Start, dfaLexerRule.TokenType);
        }
    }
}