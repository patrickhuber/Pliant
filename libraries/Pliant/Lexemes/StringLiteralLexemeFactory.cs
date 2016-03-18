using Pliant.Grammars;
using System;

namespace Pliant.Lexemes
{
    public class StringLiteralLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType
        {
            get { return StringLiteralLexerRule.StringLiteralLexerRuleType; }
        }

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create StringLiteralLexeme from type {lexerRule.GetType().FullName}. Expected StringLiteralLexerRule");
            var stringLiteralLexerRule = lexerRule as IStringLiteralLexerRule;
            return new StringLiteralLexeme(stringLiteralLexerRule);
        }
    }
}