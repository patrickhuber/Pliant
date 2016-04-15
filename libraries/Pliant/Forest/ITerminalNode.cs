namespace Pliant.Forest
{
    public interface ITerminalNode : INode
    {
        char Capture { get; }
    }
}