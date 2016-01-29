namespace Pliant.Grammars
{
    public interface INonTerminal : ISymbol
    {
        string Value { get; }
        string Namespace { get; }
        string Name { get; }
    }
}