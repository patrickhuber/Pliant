using Pliant.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Languages.Pdl
{
    public class PdlLexerRuleExpression : PdlNode
    {
        public PdlLexerRuleTerm Term { get; private set;  }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlLexerRuleExpression; } }

        private readonly int _hashCode;

        public PdlLexerRuleExpression(PdlLexerRuleTerm term)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlLexerRuleExpression expression))
                return false;
            return expression.NodeType == NodeType
                && expression.Term.Equals(Term);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class PdlLexerRuleExpressionAlteration : PdlLexerRuleExpression
    {
        private readonly int _hashCode;

        public PdlLexerRuleExpression Expression { get; private set; }

        public override PdlNodeType NodeType { get { return PdlNodeType.PdlLexerRuleExpressionAlteration; } }

        public PdlLexerRuleExpressionAlteration(PdlLexerRuleTerm term, PdlLexerRuleExpression expression)
            : base(term)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlLexerRuleExpressionAlteration expression))
                return false;
            return expression.NodeType == NodeType
                && expression.Term.Equals(Term)
                && expression.Expression.Equals(Expression);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Term.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
