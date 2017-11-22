using Pliant.Builders.Expressions;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Grammars
{
    public class RightRecursionGrammar : GrammarWrapper
    {
        private static IGrammar _grammar;

        static RightRecursionGrammar()
        {
            ProductionExpression A = "A";
            A.Rule =
                'a' + A
                | (Expr)null;

            _grammar = new GrammarExpression(A, new[] { A })
                .ToGrammar();
        }

        public RightRecursionGrammar() : base(_grammar)
        {
        }
    }
}
