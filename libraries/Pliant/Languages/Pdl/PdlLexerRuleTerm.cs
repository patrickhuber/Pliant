using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{
    public class PdlLexerRuleTerm : PdlNode
    {
        public PdlLexerRuleFactor Factor { get; private set; }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlLexerRuleTerm; } }

        readonly int _hashCode;

        public PdlLexerRuleTerm(PdlLexerRuleFactor factor)
        {
            Factor = factor;
            _hashCode = ComputeHashCode();
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(((int)NodeType).GetHashCode(), Factor.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlLexerRuleTerm term))
                return false;

            return term.NodeType == NodeType
                && term.Factor.Equals(Factor);
        }
    }

    public class PdlLexerRuleTermConcatenation : PdlLexerRuleTerm
    {
        public PdlLexerRuleTerm Term { get; private set; }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlLexerRuleTermConcatenation; } }

        readonly int _hashCode;

        public PdlLexerRuleTermConcatenation(PdlLexerRuleFactor factor, PdlLexerRuleTerm term)
            : base(factor)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(((int)NodeType).GetHashCode(), Factor.GetHashCode(), Term.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlLexerRuleTermConcatenation term))
                return false;

            return term.NodeType == NodeType
                && term.Factor.Equals(Factor)
                && term.Term.Equals(Term);
        }
    }
}