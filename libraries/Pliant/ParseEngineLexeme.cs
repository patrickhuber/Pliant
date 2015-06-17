using Pliant.Grammars;
using Pliant.Lexemes;
using Pliant.Tokens;
using System.Linq;
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
            // get expected lexemes
            var expectedLexemes  =  
                from rule in _parseEngine.GetExpectedLexerRules()
                where rule.LexerRuleType == TerminalLexerRule.TerminalLexerRuleType
                let terminalRule = rule as ITerminalLexerRule
                select new TerminalLexeme(terminalRule);

            // filter on first rule to pass (since all rules are one character per lexeme)
            var firstPassingRule = expectedLexemes.FirstOrDefault(x => x.Scan(c));
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
