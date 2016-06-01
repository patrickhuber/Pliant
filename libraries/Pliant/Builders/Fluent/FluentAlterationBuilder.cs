using Pliant.Grammars;

namespace Pliant.Builders.Fluent
{
    public class FluentAlterationBuilder
    {
        public FluentAlterationBuilder Or(params ISymbol[] symbols)
        {
            return this;
        }
    }
}