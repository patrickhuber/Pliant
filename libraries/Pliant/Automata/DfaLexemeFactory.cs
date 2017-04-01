using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class DfaLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return DfaLexerRule.DfaLexerRuleType; } }

        private Queue<DfaLexeme> _queue;

        public DfaLexemeFactory()
        {
            _queue = new Queue<DfaLexeme>();
        }

        public ILexeme Create(ILexerRule lexerRule, int position)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create DfaLexeme from type {lexerRule.GetType().FullName}. Expected DfaLexerRule");
            var dfaLexerRule = lexerRule as IDfaLexerRule;
            if (_queue.Count > 0)
            {
                var reusedLexeme = _queue.Dequeue();
                reusedLexeme.Reset(dfaLexerRule, position);
                return reusedLexeme;
            }
            var dfaLexeme = new DfaLexeme(dfaLexerRule, position);
            return dfaLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            var dfaLexeme = lexeme as DfaLexeme;
            if (dfaLexeme == null)
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} with DfaLexemeFactory");
            _queue.Enqueue(dfaLexeme);
        }
    }
}