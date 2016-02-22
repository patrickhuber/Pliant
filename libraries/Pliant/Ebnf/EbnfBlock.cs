using System;

namespace Pliant.Ebnf
{
    public abstract class EbnfBlock : EbnfNode
    {
    }

    public class EbnfBlockRule : EbnfBlock
    {
        private readonly Lazy<int> _hashCode;

        public EbnfRule Rule { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockRule; } }

        public EbnfBlockRule(EbnfRule rule)
        {
            Rule = rule;
            _hashCode = new Lazy<int>(ComputeHash);
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

        int ComputeHash()
        {
            return HashUtil.ComputeHash(
                Rule.GetHashCode(), 
                NodeType.GetHashCode());
        }
                
        public override int GetHashCode()
        {
            return _hashCode.Value;
        }
    }

    public class EbnfBlockSetting : EbnfBlock
    {
        private readonly Lazy<int> _hashCode;
        public EbnfSetting Setting { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockSetting; } }

        public EbnfBlockSetting(EbnfSetting setting)
        {
            Setting = setting;
            _hashCode = new Lazy<int>(ComputeHash);
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

        int ComputeHash()
        {
            return HashUtil.ComputeHash(
                Setting.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }
    }

    public class EbnfBlockLexerRule : EbnfBlock
    {
        private readonly Lazy<int> _hashCode;
        public EbnfLexerRule LexerRule { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockLexerRule; } }

        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
        {
            LexerRule = lexerRule;
            _hashCode = new Lazy<int>(ComputeHash);
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

        int ComputeHash()
        {
            return HashUtil.ComputeHash(
                LexerRule.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }
    }
}
