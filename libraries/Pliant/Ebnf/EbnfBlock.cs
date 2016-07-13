using Pliant.Utilities;
using System;

namespace Pliant.Ebnf
{
    public abstract class EbnfBlock : EbnfNode
    {
    }

    public class EbnfBlockRule : EbnfBlock
    {
        private readonly int _hashCode;

        public EbnfRule Rule { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockRule; } }

        public EbnfBlockRule(EbnfRule rule)
        {
            Rule = rule;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var blockRule = obj as EbnfBlockRule;
            if ((object)blockRule == null)
                return false;
            return blockRule.NodeType == NodeType
                && blockRule.Rule.Equals(Rule);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Rule.GetHashCode(), 
                NodeType.GetHashCode());
        }
                
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class EbnfBlockSetting : EbnfBlock
    {
        private readonly int _hashCode;
        public EbnfSetting Setting { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockSetting; } }

        public EbnfBlockSetting(EbnfSetting setting)
        {
            Setting = setting;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var blockRule = obj as EbnfBlockSetting;
            if ((object)blockRule == null)
                return false;
            return blockRule.NodeType == NodeType
                && blockRule.Setting.Equals(Setting);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Setting.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class EbnfBlockLexerRule : EbnfBlock
    {
        private readonly int _hashCode;
        public EbnfLexerRule LexerRule { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockLexerRule; } }

        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
        {
            LexerRule = lexerRule;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var blockRule = obj as EbnfBlockLexerRule;
            if ((object)blockRule == null)
                return false;
            return blockRule.NodeType == NodeType
                && blockRule.LexerRule.Equals(LexerRule);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                LexerRule.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
