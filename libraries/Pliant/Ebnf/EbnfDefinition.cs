using System;

namespace Pliant.Ebnf
{
    public class EbnfDefinition : EbnfNode
    {
        public EbnfBlock Block { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfDefinition; } }
    }

    public class EbnfDefinitionRepetition : EbnfDefinition
    {
        public EbnfDefinition Definition { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfDefinitionRepetition; } }
    }
}