using System;

namespace Pliant.Ebnf
{
    public class EbnfSetting : EbnfNode
    {
        public EbnfSettingIdentifier SettingIdentifier { get; private set; }
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        
        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfSetting;
            }
        }

        public EbnfSetting(EbnfSettingIdentifier settingIdentifier, EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            SettingIdentifier = settingIdentifier;
            QualifiedIdentifier = qualifiedIdentifier;
        }
    }
}