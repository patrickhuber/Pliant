using Pliant.Charts;
using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface IIntermediateForestNode : IInternalForestNode
    {
        IDottedRule DottedRule { get; }
    }
}