using System;

namespace Pliant.Ebnf
{
    public class EbnfQualifiedIdentifier : EbnfNode
    {
        public string Value { get; private set; }

        public EbnfQualifiedIdentifier(string value)
        {
            Value = value;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfSettingIdentifier;
            }
        }
    }
}