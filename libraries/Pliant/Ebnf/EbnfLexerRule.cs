using System;

namespace Pliant.Ebnf
{
    public class EbnfLexerRule : EbnfNode
    {
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public EbnfExpression Expression { get; private set; }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfLexerRule;
            }
        }

        public EbnfLexerRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
        }
    }
}