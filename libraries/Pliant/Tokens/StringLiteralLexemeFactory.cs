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

        public ILexeme Create(ILexerRule lexerRule, int position)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create StringLiteralLexeme from type {lexerRule.GetType().FullName}. Expected StringLiteralLexerRule");
            var stringLiteralLexerRule = lexerRule as IStringLiteralLexerRule;

            if (_queue.Count == 0)
                return new StringLiteralLexeme(stringLiteralLexerRule, position);
            
            var reusedLexeme = _queue.Dequeue();
            reusedLexeme.Reset(stringLiteralLexerRule, position);
            return reusedLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            var stringLiteralLexeme = lexeme as StringLiteralLexeme;
            if (stringLiteralLexeme == null)
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} from StringLiteralLexemeFactory.");
            _queue.Enqueue(stringLiteralLexeme);
        }
        
    }
}