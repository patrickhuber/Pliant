namespace Pliant.Grammars
{
    public interface IDottedRule
    {
        int Position { get; }
        IProduction Production { get; }
    }
}