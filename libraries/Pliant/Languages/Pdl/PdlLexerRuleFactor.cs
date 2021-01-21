using Pliant.Captures;
using Pliant.Languages.Regex;
using Pliant.Utilities;
using System;

namespace Pliant.Languages.Pdl
{
    public abstract class PdlLexerRuleFactor : PdlNode
    {     
    }

    public class PdlLexerRuleFactorLiteral : PdlLexerRuleFactor
    {
        public override PdlNodeType NodeType { get { return PdlNodeType.PdlLexerRuleFactorLiteral; } }

        public ICapture<char> Value { get; private set; }

        private readonly int _hashCode;

        public PdlLexerRuleFactorLiteral(string value)
            : this(value.AsCapture())
        { }

        public PdlLexerRuleFactorLiteral(ICapture<char> value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(((int)NodeType).GetHashCode(), Value.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlLexerRuleFactorLiteral factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }
    }

    public class PdlLexerRuleFactorRegex : PdlLexerRuleFactor
    {
        public override PdlNodeType NodeType { get { return PdlNodeType.PdlLexerRuleFactorRegex; } }
        private readonly int _hashCode;

        public RegexDefinition Regex { get; private set; }

        public PdlLexerRuleFactorRegex(RegexDefinition regex)
        {
            Regex = regex;
            _hashCode = ComputeHashCode();
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Regex.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorRegex factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Regex.Equals(Regex);
        }
    }
}