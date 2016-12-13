using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public class TerminalLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return TerminalLexerRule.TerminalLexerRuleType; } }
        private Queue<TerminalLexeme> _queue;

        public TerminalLexemeFactory()
        {
            _queue = new Queue<TerminalLexeme>();
        }

        public ILexeme Create(ILexerRule lexerRule, int position)
        {
            if (!LexerRuleType.Equals(lexerRule.LexerRuleType))
                throw new Exception(
                    $"Unable to create TerminalLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");
            
            var terminalLexerRule = lexerRule as ITerminalLexerRule;
            if (_queue.Count == 0)
                return new TerminalLexeme(terminalLexerRule, position);
            
            var reusedLexeme = _queue.Dequeue();
            reusedLexeme.Reset(terminalLexerRule, position);
            return reusedLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            var terminalLexeme = lexeme as TerminalLexeme;
            if (terminalLexeme == null)
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} from TerminalLexemeFactory");
            
            _queue.Enqueue(terminalLexeme);
        }
    }
}