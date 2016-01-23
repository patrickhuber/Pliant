namespace Pliant.Builders
{
    public interface IRuleBuilder
    {
        IRuleBuilder Rule(params object[] symbols);

        IRuleBuilder Lambda();
    }
}