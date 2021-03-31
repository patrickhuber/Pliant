using Pliant.Captures;
using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{
    public class PdlSettingIdentifier : PdlNode
    {
        public ICapture<char> Value { get; private set; }

        private readonly int _hashCode;

        public PdlSettingIdentifier(string value)
            : this(value.AsCapture())
        { }

        public PdlSettingIdentifier(ICapture<char> value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlSettingIdentifier;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Value.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlSettingIdentifier factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }
    }
}