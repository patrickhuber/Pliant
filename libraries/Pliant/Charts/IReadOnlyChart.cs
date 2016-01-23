using System.Collections.Generic;

namespace Pliant.Charts
{
    public interface IReadOnlyChart
    {
        IReadOnlyList<IEarleySet> EarleySets { get; }
        int Count { get; }
    }
}