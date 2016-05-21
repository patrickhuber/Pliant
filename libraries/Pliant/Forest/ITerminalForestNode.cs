namespace Pliant.Forest
{
    public interface ITerminalForestNode : IForestNode
    {
        char Capture { get; }
    }
}