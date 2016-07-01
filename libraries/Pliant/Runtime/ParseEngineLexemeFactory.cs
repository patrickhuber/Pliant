using Pliant.Grammars;
using Pliant.Lexemes;
using System;

namespace Pliant.Runtime
{
    public class ParseEngineLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return GrammarLexerRule.GrammarLexerRuleType; } }

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create ParseEngineLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");

            var grammarLexerRule = lexerRule as IGrammarLexerRule;
            var parseEngine = new ParseEngine(grammarLexerRule.Grammar);

            return new ParseEngineLexeme(parseEngine, grammarLexerRule.TokenType);
        }
    }
}