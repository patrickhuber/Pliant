using Pliant.Grammars;
using System;

namespace Pliant.Lexemes
{
    public class TerminalLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return TerminalLexerRule.TerminalLexerRuleType; } }

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create TerminalLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");
            var terminalLexerRule = lexerRule as ITerminalLexerRule;
            return new TerminalLexeme(terminalLexerRule);
        }
    }
}