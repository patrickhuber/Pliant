using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IEarleme
    {
        IReadOnlyList<IState> Predictions { get; }
        IReadOnlyList<IState> Scans { get; }
        IReadOnlyList<IState> Completions { get; }
        IReadOnlyList<IState> Transitions { get; }
        bool Enqueue(IState state);
    }
}
