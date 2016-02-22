using System;

namespace Pliant.Ebnf
{
    public abstract class EbnfFactor : EbnfNode
    {
    }

    public class EbnfFactorIdentifier : EbnfFactor
    {
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public EbnfFactorIdentifier(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorIdentifier;
            }
        }
    }

    public class EbnfFactorLiteral : EbnfFactor
    {
        public string Value { get; private set; }

        public EbnfFactorLiteral(string value)
        {
            Value = value;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorLiteral;
            }
        }
    }

    public class EbnfFactorRegex : EbnfFactor
    {
        public RegularExpressions.Regex Regex { get; private set; }

        public EbnfFactorRegex(RegularExpressions.Regex regex)
        {
            Regex = regex;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorRegex;
            }
        }
    }

    public class EbnfFactorRepetition : EbnfFactor
    {
        public EbnfExpression Expression { get; private set; }

        public EbnfFactorRepetition(EbnfExpression expression)
        {
            Expression = expression;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorRepetition;
            }
        }
    }

    public class EbnfFactorOptional : EbnfFactor
    {
        public EbnfExpression Expression { get; private set; }

        public EbnfFactorOptional(EbnfExpression expression)
        {
            Expression = expression;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorOptional;
            }
        }
    }

    public class EbnfFactorGrouping : EbnfFactor
    {
        public EbnfExpression Expression { get; private set; }

        public EbnfFactorGrouping(EbnfExpression expression)
        {
            Expression = expression;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfFactorGrouping;
            }
        }
    }
}