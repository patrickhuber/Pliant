using System;

namespace Pliant.Ebnf
{
    public class EbnfDefinition : EbnfNode
    {
        public EbnfDefinition(EbnfBlock block)
        {
            Block = block;
        }

        public EbnfBlock Block { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfDefinition; } }
    }

    public class EbnfDefinitionRepetition : EbnfDefinition
    {        
        public EbnfDefinitionRepetition(EbnfBlock block, EbnfDefinition definition)
            : base(block)
        {
            Definition = definition;
        }

        public EbnfDefinition Definition { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfDefinitionRepetition; } }
    }
}