using Pliant.Grammars;

using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Text;
using System;

namespace Pliant.Runtime
{
    public class ParseEngineLexeme : ILexeme
    {
        public string Value { get { return _capture.ToString(); } }

        public TokenType TokenType { get { return LexerRule.TokenType; } }

        public ILexerRule LexerRule { get; private set; }

        public int Position { get; private set; }

        private StringBuilder _capture;
        private IParseEngine _parseEngine;

        public ParseEngineLexeme(IGrammarLexerRule lexerRule)
        {
            _capture = new StringBuilder();
            _parseEngine = new ParseEngine(lexerRule.Grammar);
            LexerRule = lexerRule;
        }

        public bool Scan(char c)
        {
            // get expected lexems
            // PERF: Avoid Linq where, let and select expressions due to lambda allocation
            var expectedLexemes = SharedPools.Default<List<TerminalLexeme>>().AllocateAndClear();
            var expectedLexerRules = _parseEngine.GetExpectedLexerRules();

            foreach (var rule in expectedLexerRules)
                if (rule.LexerRuleType == TerminalLexerRule.TerminalLexerRuleType)
                    expectedLexemes.Add(new TerminalLexeme(rule as ITerminalLexerRule, Position));

            // filter on first rule to pass (since all rules are one character per lexeme)
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            TerminalLexeme firstPassingRule = null;
            foreach (var lexeme in expectedLexemes)
                if (lexeme.Scan(c))
                {
                    firstPassingRule = lexeme;
                    break;
                }
            SharedPools.Default<List<TerminalLexeme>>()
                .ClearAndFree(expectedLexemes);

            if (firstPassingRule == null)
                return false;

            var token = new Token(firstPassingRule.Value, _parseEngine.Location, firstPassingRule.TokenType);

            var result = _parseEngine.Pulse(token);
            if (result)
                _capture.Append(c);

            return result;
        }

        public bool IsAccepted()
        {
            return _parseEngine.IsAccepted();
        }

        public void Reset(IGrammarLexerRule newGrammarRule, int position)
        {
            LexerRule = newGrammarRule;
            _capture.Clear();

            _parseEngine = new ParseEngine(newGrammarRule.Grammar);
            Position = position;
        }
    }
}