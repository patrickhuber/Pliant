using Pliant.Grammars;
using Pliant.Lexemes;
using System;

namespace Pliant
{
    public class ParseEngineLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return GrammarLexerRule.GrammarLexerRuleType; } }

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    string.Format(
                        "Unable to create ParseEngineLexeme from type {0}. Expected TerminalLexerRule",
                        lexerRule.GetType().FullName));

            var grammarLexerRule = lexerRule as IGrammarLexerRule;
            var parseEngine = new ParseEngine(grammarLexerRule.Grammar);

            return new ParseEngineLexeme(parseEngine, grammarLexerRule.TokenType);
        }
    }
}