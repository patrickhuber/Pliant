using System;

namespace Pliant.Ebnf
{
    public abstract class EbnfFactor : EbnfNode
    {
    }

    public class EbnfFactorIdentifier : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public EbnfFactorIdentifier(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorIdentifier;
            }
        }

        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfFactorIdentifier;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }
    }

    public class EbnfFactorLiteral : EbnfFactor
    {
        private readonly int _hashCode;

        public string Value { get; private set; }

        public EbnfFactorLiteral(string value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorLiteral;
            }
        }
        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Value.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfFactorLiteral;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }
    }

    public class EbnfFactorRegex : EbnfFactor
    {
        private readonly int _hashCode;

        public RegularExpressions.Regex Regex { get; private set; }

        public EbnfFactorRegex(RegularExpressions.Regex regex)
        {
            Regex = regex;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorRegex;
            }
        }
        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Regex.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfFactorRegex;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.Regex.Equals(Regex);
        }
    }

    public class EbnfFactorConcatenation : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfFactorConcatenation(EbnfExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorConcatenation;
            }
        }
        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfFactorConcatenation;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }
    }

    public class EbnfFactorOptional : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfFactorOptional(EbnfExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorOptional;
            }
        }
        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfFactorOptional;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }
    }

    public class EbnfFactorGrouping : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfFactorGrouping(EbnfExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorGrouping;
            }
        }
        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as EbnfFactorGrouping;
            if ((object)factor == null)
                return false;
            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }
    }
}