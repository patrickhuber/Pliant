using Pliant.Charts;

namespace Pliant.Forest
{
    public interface IForestNode : IForestNodeVisitable, IParseNode
    {
        int Origin { get; }

        int Location { get; }

        ForestNodeType NodeType { get; }
    }
}