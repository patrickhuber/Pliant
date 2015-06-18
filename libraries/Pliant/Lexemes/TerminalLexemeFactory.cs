using System;
using Pliant.Grammars;

namespace Pliant.Lexemes
{
    public class TerminalLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return TerminalLexerRule.TerminalLexerRuleType; } }

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    string.Format(
                        "Unable to create TerminalLexeme from type {0}. Expected TerminalLexerRule", 
                        lexerRule.GetType().FullName));
            var terminalLexerRule = lexerRule as ITerminalLexerRule;
            return new TerminalLexeme(terminalLexerRule);
        }
    }
}
