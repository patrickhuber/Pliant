using Pliant.Builders.Expressions;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Grammars
{
    public class NullableGrammar : GrammarWrapper
    {
        private static readonly IGrammar _grammar;

        static NullableGrammar()
        {
            ProductionExpression
                SP = nameof(SP),
                S = nameof(S),
                A = nameof(A),
                E = nameof(E);

            SP.Rule = S;
            S.Rule = A + A + A + A;
            A.Rule = 'a' | E;
            E.Rule = null;

            _grammar = new GrammarExpression(SP).ToGrammar();
        }

        public NullableGrammar() 
            : base(_grammar)
        {
        }
    }
}
