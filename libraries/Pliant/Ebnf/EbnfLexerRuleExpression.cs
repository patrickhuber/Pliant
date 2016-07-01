using Pliant.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Ebnf
{
    public class EbnfLexerRuleExpression : EbnfNode
    {
        public EbnfLexerRuleTerm Term { get; private set;  }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfLexerRuleExpression; } }

        private readonly int _hashCode;

        public EbnfLexerRuleExpression(EbnfLexerRuleTerm term)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var expression = obj as EbnfLexerRuleExpression;
            if ((object)expression == null)
                return false;
            return expression.NodeType == NodeType
                && expression.Term.Equals(Term);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }

    public class EbnfLexerRuleExpressionAlteration : EbnfLexerRuleExpression
    {
        private readonly int _hashCode;

        public EbnfLexerRuleExpression Expression { get; private set; }

        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfLexerRuleExpressionAlteration; } }

        public EbnfLexerRuleExpressionAlteration(EbnfLexerRuleTerm term, EbnfLexerRuleExpression expression)
            : base(term)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var expression = obj as EbnfLexerRuleExpressionAlteration;
            if ((object)expression == null)
                return false;
            return expression.NodeType == NodeType
                && expression.Term.Equals(Term)
                && expression.Expression.Equals(Expression);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Term.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
