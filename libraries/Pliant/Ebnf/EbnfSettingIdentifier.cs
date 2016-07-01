using Pliant.Utilities;
using System;

namespace Pliant.Ebnf
{
    public class EbnfSettingIdentifier : EbnfNode
    {
        public string Value { get; private set; }

        private readonly int _hashCode;

        public EbnfSettingIdentifier(string value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfSettingIdentifier;
            }
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Value.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfSettingIdentifier;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }
    }
}