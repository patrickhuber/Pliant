using System;
namespace Pliant
{
    public interface IRuleBuilder
    {
        IRuleBuilder Rule(params object[] symbols);
        IRuleBuilder Lambda();
    }
}
