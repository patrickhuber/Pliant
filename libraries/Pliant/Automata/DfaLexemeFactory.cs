using Pliant.Grammars;
using Pliant.Lexemes;
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

        public ILexeme Create(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create DfaLexeme from type {lexerRule.GetType().FullName}. Expected DfaLexerRule");
            var dfaLexerRule = lexerRule as IDfaLexerRule;
            if (_queue.Count > 0)
            {
                var reusedLexeme = _queue.Dequeue();
                reusedLexeme.Reset(dfaLexerRule);
                return reusedLexeme;
            }
            return new DfaLexeme(dfaLexerRule.Start, dfaLexerRule.TokenType);
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