using Pliant.Charts;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Forest
{
    public class FakeIntermediateForestNode : FakeInternalForestNode, IIntermediateForestNode
    {
        public FakeIntermediateForestNode(IDottedRule dottedRule, int origin, int location, params IAndForestNode[] children) 
            : base(origin, location, children)
        {
            DottedRule = dottedRule;
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Intermediate; }
        }

        public IDottedRule DottedRule { get; private set; }
    }
}
