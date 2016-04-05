using Pliant.Charts;

namespace Pliant.Forest
{
    public interface IIntermediateNode : IInternalNode
    {
        IState State { get; }
    }
}