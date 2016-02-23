﻿using System;

namespace Pliant.Ebnf
{
    public class EbnfDefinition : EbnfNode
    {
        private readonly Lazy<int> _hashCode;

        public EbnfDefinition(EbnfBlock block)
        {
            Block = block;
            _hashCode = new Lazy<int>(ComputeHashCode);
        }


        public EbnfBlock Block { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfDefinition; } }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var ebnfDefinition = obj as EbnfDefinition;
            if ((object)ebnfDefinition == null)
                return false;
                
            return ebnfDefinition.NodeType == NodeType 
                && ebnfDefinition.Block.Equals(Block);
        }
        
        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                Block.GetHashCode(), 
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }
    }

    public class EbnfDefinitionConcatenation : EbnfDefinition
    {
        private readonly Lazy<int> _hashCode;

        public EbnfDefinitionConcatenation(EbnfBlock block, EbnfDefinition definition)
            : base(block)
        {
            Definition = definition;
            _hashCode = new Lazy<int>(ComputeHashCode);
        }

        public EbnfDefinition Definition { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfDefinitionConcatenation; } }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var ebnfDefinitionRepetition = obj as EbnfDefinitionConcatenation;
            if ((object)ebnfDefinitionRepetition == null)
                return false;
            
            return ebnfDefinitionRepetition.NodeType == EbnfNodeType.EbnfDefinitionConcatenation
                && ebnfDefinitionRepetition.Block.Equals(Block)
                && ebnfDefinitionRepetition.Definition.Equals(Definition);
        }

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                Block.GetHashCode(),
                Definition.GetHashCode(),
                NodeType.GetHashCode());
        }
        
        public override int GetHashCode()
        {
            return _hashCode.Value;
        }
    }
}