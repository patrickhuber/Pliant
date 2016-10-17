using Pliant.Builders.Expressions;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Grammars
{
    public class CycleGrammar : GrammarWrapper
    {
        private static IGrammar _grammar;

        static CycleGrammar()
        {
            ProductionExpression
                A = nameof(A),
                B = nameof(B),
                C = nameof(C);

            A.Rule = B | 'a';
            B.Rule = C | 'b';
            C.Rule = A | 'c';

            _grammar = new GrammarExpression(A, new[] { A, B, C })
                .ToGrammar();
        }

        public CycleGrammar() : base(_grammar)
        {
        }
    }
}
