using Pliant.Utilities;
using System;

namespace Pliant.Ebnf
{
    public class EbnfSetting : EbnfNode
    {
        private readonly int _hashCode;

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
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var qualifiedIdentifier = obj as EbnfSetting;
            if ((object)qualifiedIdentifier == null)
                return false;
            return qualifiedIdentifier.NodeType == NodeType
                && qualifiedIdentifier.SettingIdentifier.Equals(SettingIdentifier)
                && qualifiedIdentifier.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                SettingIdentifier.GetHashCode(),
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

    }
}