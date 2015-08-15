using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class TerminalLexerRule : ITerminalLexerRule
    {
        public ITerminal Terminal { get; }
        
        public SymbolType SymbolType { get { return SymbolType.LexerRule; } }

        public TokenType TokenType { get; private set; }
        
        public static readonly LexerRuleType TerminalLexerRuleType = new LexerRuleType("Terminal");

        public LexerRuleType LexerRuleType { get { return TerminalLexerRuleType; } }

        public TerminalLexerRule(char character)
            : this(new Terminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexerRule(ITerminal terminal, TokenType tokenType)
        {
            Terminal = terminal;
            TokenType = tokenType;
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
