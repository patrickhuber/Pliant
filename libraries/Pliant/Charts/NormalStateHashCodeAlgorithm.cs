using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    static class NormalStateHashCodeAlgorithm
    {
        public static int Compute(IDottedRule dottedRule, int origin)
        {
            return HashCode.Compute(
                dottedRule.Position.GetHashCode(),
                origin.GetHashCode(),
                dottedRule.Production.GetHashCode());
        }
    }
}
