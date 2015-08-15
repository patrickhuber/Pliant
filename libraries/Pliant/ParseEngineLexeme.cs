using Pliant.Grammars;
using Pliant.Lexemes;
using Pliant.Tokens;
using System.Collections.Generic;
using System.Text;

namespace Pliant
{
    public class ParseEngineLexeme : ILexeme
    {
        public string Capture { get { return _capture.ToString(); } }

        public TokenType TokenType { get; private set; }

        private StringBuilder _capture;
        private IParseEngine _parseEngine;

        public ParseEngineLexeme(IParseEngine parseEngine, TokenType tokenType)
        {
            TokenType = tokenType;
            _capture = new StringBuilder();
            _parseEngine = parseEngine;
        }
                
        public bool Scan(char c)
        {
            // get expected lexems
            // PERF: Avoid Linq where, let and select expressions due to lambda allocation
            var expectedLexemes = new List<TerminalLexeme>();
            foreach (var rule in _parseEngine.GetExpectedLexerRules())
                if (rule.LexerRuleType == TerminalLexerRule.TerminalLexerRuleType)
                    expectedLexemes.Add(new TerminalLexeme(rule as ITerminalLexerRule));
            
            // filter on first rule to pass (since all rules are one character per lexeme)
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            TerminalLexeme firstPassingRule = null;
            foreach (var lexeme in expectedLexemes)
                if (lexeme.Scan(c))
                {
                    firstPassingRule = lexeme;
                    break;
                }

            if (firstPassingRule == null)
                return false;

            var token = new Token(firstPassingRule.Capture, _parseEngine.Location, firstPassingRule.TokenType);

            var result = _parseEngine.Pulse(token);
            if (result)
                _capture.Append(c);

            return result;
        }
        
        public bool IsAccepted()
        {
            return _parseEngine.IsAccepted();
        }
    }
}
