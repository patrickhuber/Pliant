using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;

namespace Pliant.Runtime
{
    public class ParseEngineLexemeFactory : ILexemeFactory
    {
        private Queue<ParseEngineLexeme> _queue;

        public LexerRuleType LexerRuleType { get { return GrammarLexerRule.GrammarLexerRuleType; } }

        public ParseEngineLexemeFactory()
        {
            _queue = new Queue<ParseEngineLexeme>();
        }

        public ILexeme Create(ILexerRule lexerRule, int position)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create ParseEngineLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");

            var grammarLexerRule = lexerRule as IGrammarLexerRule;

            if (_queue.Count == 0)
                return new ParseEngineLexeme(grammarLexerRule);
            
            var reusedLexeme = _queue.Dequeue();
            reusedLexeme.Reset(grammarLexerRule, position);
            return reusedLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            var parseEngineLexeme = lexeme as ParseEngineLexeme;
            if(parseEngineLexeme == null)
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} from ParseEngineLexeme.");
            _queue.Enqueue(parseEngineLexeme);
        }
    }
}