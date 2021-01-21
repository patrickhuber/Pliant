using Pliant.Grammars;

using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using Pliant.Captures;

namespace Pliant.Runtime
{
    public class ParseEngineLexeme : LexemeBase<IGrammarLexerRule>, ILexeme
    {                
        private IParseEngine _parseEngine;

        public ParseEngineLexeme(IGrammarLexerRule lexerRule, ICapture<char> segment, int offset)
            : base(lexerRule, segment, offset)
        {            
            _parseEngine = new ParseEngine(lexerRule.Grammar);
        }

        public override bool Scan()
        {
            // get expected lexems
            // PERF: Avoid Linq where, let and select expressions due to lambda allocation
            var expectedLexemes = SharedPools.Default<List<TerminalLexeme>>().AllocateAndClear();
            var expectedLexerRules = _parseEngine.GetExpectedLexerRules();

            foreach (var rule in expectedLexerRules)
                if (rule.LexerRuleType == TerminalLexerRule.TerminalLexerRuleType)
                    expectedLexemes.Add(
                        new TerminalLexeme(
                            rule as ITerminalLexerRule, 
                            Capture,
                            _parseEngine.Location));

            // filter on first rule to pass (since all rules are one character per lexeme)
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            TerminalLexeme firstPassingRule = null;
            foreach (var lexeme in expectedLexemes)
                if (lexeme.Scan())
                {
                    firstPassingRule = lexeme;
                    break;
                }
            SharedPools.Default<List<TerminalLexeme>>()
                .ClearAndFree(expectedLexemes);

            if (firstPassingRule is null)
                return false;

            var result = _parseEngine.Pulse(firstPassingRule);
            if (!result)
                return false;

            return Capture.Grow();
        }

        public override bool IsAccepted()
        {
            return _parseEngine.IsAccepted();
        }

        public override void Reset()
        {            
            _parseEngine = new ParseEngine(ConcreteLexerRule.Grammar);
        }
    }
}