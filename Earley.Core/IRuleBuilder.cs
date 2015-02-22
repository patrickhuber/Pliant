using System;
namespace Earley
{
    public interface IRuleBuilder
    {
        IRuleBuilder Rule(params object[] symbols);
    }
}
