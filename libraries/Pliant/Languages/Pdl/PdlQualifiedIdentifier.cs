using Pliant.Captures;
using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{
    public class PdlQualifiedIdentifier : PdlNode
    {
        private readonly int _hashCode;

        public ICapture<char> Identifier { get; private set; }

        public PdlQualifiedIdentifier(string identifier)
            : this(identifier.AsCapture())
        { 
        }

        public PdlQualifiedIdentifier(ICapture<char> identifier)
        {
            Assert.IsNotNull(identifier, nameof(identifier));
            Identifier = identifier;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlQualifiedIdentifier;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlQualifiedIdentifier qualifiedIdentifier))
                return false;
            return qualifiedIdentifier.NodeType == NodeType
                && qualifiedIdentifier.Identifier.Equals(Identifier);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(((int)NodeType).GetHashCode(), Identifier.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override string ToString() => Identifier.ToString();
    }

    public class PdlQualifiedIdentifierConcatenation : PdlQualifiedIdentifier
    {
        private readonly int _hashCode;

        public PdlQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public PdlQualifiedIdentifierConcatenation(
            string identifier,
            PdlQualifiedIdentifier qualifiedIdentifier)
            : this(identifier.AsCapture(),
                  qualifiedIdentifier)
        { }

        public PdlQualifiedIdentifierConcatenation(
            ICapture<char> identifier,
            PdlQualifiedIdentifier qualifiedIdentifier)
            : base(identifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlQualifiedIdentifierConcatenation;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlQualifiedIdentifierConcatenation qualifiedIdentifier))
                return false;
            return qualifiedIdentifier.NodeType == NodeType
                && qualifiedIdentifier.Identifier.Equals(Identifier)
                && qualifiedIdentifier.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Identifier.GetHashCode(),
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override string ToString() => $"{QualifiedIdentifier}.{Identifier}";
    }
}