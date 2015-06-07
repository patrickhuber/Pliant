namespace Pliant.Nodes
{
    public interface IIntermediateNode : IInternalNode
    {
        IState State { get; }
    }
}
