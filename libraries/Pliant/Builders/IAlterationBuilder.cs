namespace Pliant.Builders
{
    public interface IAlterationBuilder
    {
        IAlterationBuilder Or(params SymbolBuilder[] rules);
    }
}