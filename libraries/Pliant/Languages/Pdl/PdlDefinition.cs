using Pliant.Diagnostics;
using Pliant.Utilities;
using System;

namespace Pliant.Languages.Pdl
{
    public class PdlDefinition : PdlNode
    {
        private readonly int _hashCode;

        public PdlBlock Block { get; private set; }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlDefinition; } }

        public PdlDefinition(PdlBlock block)
        {
            Assert.IsNotNull(block, nameof(block));
            Block = block;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlDefinition definition))
                return false;

            return definition.NodeType == NodeType
                && definition.Block.Equals(Block);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Block.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class PdlDefinitionConcatenation : PdlDefinition
    {
        private readonly int _hashCode;

        public PdlDefinition Definition { get; private set; }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlDefinitionConcatenation; } }

        public PdlDefinitionConcatenation(PdlBlock block, PdlDefinition definition)
            : base(block)
        {
            Assert.IsNotNull(definition, nameof(definition));
            Definition = definition;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlDefinitionConcatenation ebnfDefinitionConcatenation))
                return false;

            return ebnfDefinitionConcatenation.NodeType == PdlNodeType.PdlDefinitionConcatenation
                && ebnfDefinitionConcatenation.Block.Equals(Block)
                && ebnfDefinitionConcatenation.Definition.Equals(Definition);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Block.GetHashCode(),
                Definition.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}