using Pliant.Utilities;
using System;

namespace Pliant.Languages.Pdl
{
    public class PdlSetting : PdlNode
    {
        private readonly int _hashCode;

        public PdlSettingIdentifier SettingIdentifier { get; private set; }
        public PdlQualifiedIdentifier QualifiedIdentifier { get; private set; }
        
        public override PdlNodeType NodeType
        {
            get
            {
                return PdlNodeType.PdlSetting;
            }
        }

        public PdlSetting(PdlSettingIdentifier settingIdentifier, PdlQualifiedIdentifier qualifiedIdentifier)
        {
            SettingIdentifier = settingIdentifier;
            QualifiedIdentifier = qualifiedIdentifier;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlSetting qualifiedIdentifier))
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