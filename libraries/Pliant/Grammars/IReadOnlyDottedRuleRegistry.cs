namespace Pliant.Grammars
{
    public interface IReadOnlyDottedRuleRegistry
    {
        IDottedRule Get(IProduction production, int position);
    }
}
