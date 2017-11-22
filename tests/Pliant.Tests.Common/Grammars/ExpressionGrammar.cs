using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;

namespace Pliant.Tests.Common.Grammars
{
    public class ExpressionGrammar : GrammarWrapper
    {
        private static readonly IGrammar _grammar;

        static ExpressionGrammar()
        {
            var number = new NumberLexerRule();

            ProductionExpression
                S = nameof(S),
                E = nameof(E),
                T = nameof(T),
                F = nameof(F);

            S.Rule = E;
            E.Rule = E + '+' + T
                | E + '-' + T
                | T;
            T.Rule = T + '*' + F
                | T + '/' + F
                | F;
            F.Rule = '+' + F
                | '-' + F
                | number
                | '(' + E + ')';

            _grammar = new GrammarExpression(S).ToGrammar();
        }

        public ExpressionGrammar() 
            : base(_grammar)
        {
        }
    }
}
