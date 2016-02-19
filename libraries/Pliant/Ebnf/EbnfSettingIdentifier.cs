using System;

namespace Pliant.Ebnf
{
    public class EbnfSettingIdentifier : EbnfNode
    {
        public string Value { get; private set; }

        public EbnfSettingIdentifier(string value)
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