using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class TerminalLexerRule : BaseLexerRule, ITerminalLexerRule
    {
        public ITerminal Terminal { get; }
        
        public static readonly LexerRuleType TerminalLexerRuleType = new LexerRuleType("Terminal");

        public TerminalLexerRule(char character)
            : this(new Terminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexerRule(ITerminal terminal, TokenType tokenType)
            : base(TerminalLexerRuleType, tokenType)
        {
            Terminal = terminal;
        }

        public TerminalLexerRule(ITerminal terminal, string tokenTypeId)
            : this(terminal, new TokenType(tokenTypeId))
        {
        }
        
        public override string ToString()
        {
            return Terminal.ToString();
        }
    }
}
