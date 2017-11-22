using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IDottedRuleRegistry : IReadOnlyDottedRuleRegistry
    {
        void Register(IDottedRule dottedRule);
        IDottedRule GetNext(IDottedRule dottedRule);
    }
}