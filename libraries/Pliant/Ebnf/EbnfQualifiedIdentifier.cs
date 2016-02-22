namespace Pliant.Ebnf
{
    public class EbnfQualifiedIdentifier : EbnfNode
    {
        public string Identifier { get; private set; }

        public EbnfQualifiedIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfQualifiedIdentifier;
            }
        }
    }

    public class EbnfQualifiedIdentifierRepetition : EbnfQualifiedIdentifier
    {
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public EbnfQualifiedIdentifierRepetition(
            string identifier,
            EbnfQualifiedIdentifier qualifiedIdentifier)
            : base(identifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfQualifiedIdentifierRepetition;
            }
        }
    }
}