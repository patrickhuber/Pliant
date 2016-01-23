namespace Pliant.Ast
{
    public interface ITerminalNode : INode
    {
        char Capture { get; }
    }
}