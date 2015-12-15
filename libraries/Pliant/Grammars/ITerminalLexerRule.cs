namespace Pliant.Grammars
{
    public interface ITerminalLexerRule : ILexerRule
    {
        ITerminal Terminal { get; }
    }
}