using Pliant.Grammars;

namespace Pliant
{
    public interface IDottedRule
    {
        int Position { get; }
        ISymbol Symbol { get; }
        bool IsComplete { get; }
        bool HasMoreTransitions { get; }
        IDottedRule NextRule();
        INullable<ISymbol> PostDotSymbol { get; }
        INullable<ISymbol> PreDotSymbol { get; }
    }
}
