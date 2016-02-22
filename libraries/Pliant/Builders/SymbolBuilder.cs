using Pliant.Grammars;

namespace Pliant.Builders
{
    public class SymbolBuilder : BaseBuilder
    {
        private ISymbol _symbol;

        public override ISymbol Symbol { get { return _symbol; } }

        public SymbolBuilder(ISymbol symbol)
        {
            _symbol = symbol;
        }

        public static implicit operator SymbolBuilder(ProductionBuilder builder)
        {
            return new SymbolBuilder(builder.LeftHandSide);
        }

        public static implicit operator SymbolBuilder(string input)
        {
            return new SymbolBuilder(new StringLiteralLexerRule(input));
        }

        public static implicit operator SymbolBuilder(char character)
        {
            return new SymbolBuilder(new TerminalLexerRule(character));
        }

        public static implicit operator SymbolBuilder(BaseTerminal terminal)
        {
            return new SymbolBuilder(new TerminalLexerRule(terminal, terminal.ToString()));
        }

        public static implicit operator SymbolBuilder(BaseLexerRule rule)
        {
            return new SymbolBuilder(rule);
        }
    }
}