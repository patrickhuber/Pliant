using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{
    public abstract class PdlBlock : PdlNode
    {
    }

    public class PdlBlockRule : PdlBlock
    {
        private readonly int _hashCode;

        public PdlRule Rule { get; private set; }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlBlockRule; } }

        public PdlBlockRule(PdlRule rule)
        {
            Rule = rule;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlBlockRule blockRule))
                return false;
            return blockRule.NodeType == NodeType
                && blockRule.Rule.Equals(Rule);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Rule.GetHashCode(),
                ((int)NodeType).GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class PdlBlockSetting : PdlBlock
    {
        private readonly int _hashCode;
        public PdlSetting Setting { get; private set; }
        public override PdlNodeType NodeType { get { return PdlNodeType.PdlBlockSetting; } }

        public PdlBlockSetting(PdlSetting setting)
        {
            Setting = setting;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlBlockSetting blockRule))
                return false;
            return blockRule.NodeType == NodeType
                && blockRule.Setting.Equals(Setting);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Setting.GetHashCode(),
                ((int)NodeType).GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class PdlBlockLexerRule : PdlBlock
    {
        private readonly int _hashCode;
        public PdlLexerRule LexerRule { get; private set; }
        public override PdlNodeType NodeType { get { return PdlNodeType.PdlBlockLexerRule; } }

        public PdlBlockLexerRule(PdlLexerRule lexerRule)
        {
            LexerRule = lexerRule;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlBlockLexerRule blockRule))
                return false;
            return blockRule.NodeType == NodeType
                && blockRule.LexerRule.Equals(LexerRule);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                LexerRule.GetHashCode(),
                ((int)NodeType).GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}