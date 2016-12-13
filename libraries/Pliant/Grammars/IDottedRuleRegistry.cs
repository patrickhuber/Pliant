namespace Pliant.Grammars
{
    public interface IDottedRuleRegistry
    {
        IDottedRule Get(IProduction production, int position);
        void Register(IDottedRule dottedRule);
    }
}