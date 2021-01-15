using Pliant.Captures;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public class StringLiteralLexemeFactory : ILexemeFactory
    {
        private Queue<StringLiteralLexeme> _queue;

        public LexerRuleType LexerRuleType
        {
            get { return StringLiteralLexerRule.StringLiteralLexerRuleType; }
        }

        public StringLiteralLexemeFactory()
        {
            _queue = new Queue<StringLiteralLexeme>();
        }

        public ILexeme Create(ILexerRule lexerRule, ICapture<char> segment, int offset)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create StringLiteralLexeme from type {lexerRule.GetType().FullName}. Expected StringLiteralLexerRule");
            var stringLiteralLexerRule = lexerRule as IStringLiteralLexerRule;

            if (_queue.Count == 0)
                return new StringLiteralLexeme(stringLiteralLexerRule, segment, offset);
            
            var reusedLexeme = _queue.Dequeue();
            reusedLexeme.Reset(stringLiteralLexerRule, offset);
            return reusedLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            if (!(lexeme is StringLiteralLexeme stringLiteralLexeme))
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} from StringLiteralLexemeFactory.");
            _queue.Enqueue(stringLiteralLexeme);
        }
    }
}