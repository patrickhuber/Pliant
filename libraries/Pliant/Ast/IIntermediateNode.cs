using Pliant.Charts;

namespace Pliant.Ast
{
    public interface IIntermediateNode : IInternalNode
    {
        IState State { get; }
    }
}