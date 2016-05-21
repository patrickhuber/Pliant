using Pliant.Charts;

namespace Pliant.Forest
{
    public interface IIntermediateForestNode : IInternalForestNode
    {
        IState State { get; }
    }
}