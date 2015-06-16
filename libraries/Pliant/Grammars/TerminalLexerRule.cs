using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class TerminalLexerRule : ITerminalLexerRule
    {
        public ITerminal Terminal { get; }
        
        public SymbolType SymbolType { get { return SymbolType.LexerRule; } }

        public TokenType TokenType { get; private set; }

        public LexerRuleType LexerRuleType { get { return LexerRuleType.Terminal; } }

        public TerminalLexerRule(char character)
            : this(new Terminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexerRule(ITerminal terminal, TokenType tokenType)
        {
            Terminal = terminal;
            TokenType = tokenType;
        }
        
        public override string ToString()
        {
            return Terminal.ToString();
        }
    }
}
