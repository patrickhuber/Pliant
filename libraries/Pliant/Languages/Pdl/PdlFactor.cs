using Pliant.Captures;
using Pliant.Languages.Regex;
using Pliant.Utilities;
using System;

namespace Pliant.Languages.Pdl
{
    public abstract class PdlFactor : PdlNode
    {
    }

    public class PdlFactorIdentifier : PdlFactor
    {
        private readonly int _hashCode;

        public PdlQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public PdlFactorIdentifier(PdlQualifiedIdentifier qualifiedIdentifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlFactorIdentifier;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorIdentifier factor))
                return false;
            return factor.NodeType == NodeType
                && factor.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        public override string ToString() => QualifiedIdentifier.ToString();
    }

    public class PdlFactorLiteral : PdlFactor
    {
        private readonly int _hashCode;

        public ICapture<char> Value { get; private set; }

        public PdlFactorLiteral(string value)
            : this(value.AsCapture())
        { 
        }

        public PdlFactorLiteral(ICapture<char> value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlFactorLiteral;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Value.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorLiteral factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }

        public override string ToString() => Value.ToString();
    }

    public class PdlFactorRegex : PdlFactor
    {
        private readonly int _hashCode;

        public RegexDefinition Regex { get; private set; }

        public PdlFactorRegex(RegexDefinition regex)
        {
            Regex = regex;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlFactorRegex;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Regex.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorRegex factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Regex.Equals(Regex);
        }

        public override string ToString() => $"/{Regex}/";
    }

    public class PdlFactorRepetition : PdlFactor
    {
        private readonly int _hashCode;

        public PdlExpression Expression { get; private set; }

        public override PdlNodeType NodeType => PdlNodeType.PdlFactorRepetition;

        public PdlFactorRepetition(PdlExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorRepetition factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }

        public override int GetHashCode() => _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Expression.GetHashCode());
        }

        public override string ToString() => $"{{{Expression}}}";
    }

    public class PdlFactorOptional : PdlFactor
    {
        private readonly int _hashCode;

        public PdlExpression Expression { get; private set; }

        public PdlFactorOptional(PdlExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlFactorOptional;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorOptional factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }

        public override string ToString() => $"[{Expression}]";
    }

    public class PdlFactorGrouping : PdlFactor
    {
        private readonly int _hashCode;

        public PdlExpression Expression { get; private set; }

        public PdlFactorGrouping(PdlExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlFactorGrouping;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlFactorGrouping factor))
                return false;
            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }

        public override string ToString() => $"({Expression})";
    }
}