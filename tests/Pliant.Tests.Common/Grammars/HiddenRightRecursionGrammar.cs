using Pliant.Builders.Expressions;
using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tests.Common.Grammars
{
    public class HiddenRightRecursionGrammar : GrammarWrapper
    {
        private static IGrammar _grammar;

        static HiddenRightRecursionGrammar()
        {
            ProductionExpression
                A = nameof(A),
                B = nameof(B),
                C = nameof(C);

            A.Rule = 'a' + B | 'a';
            B.Rule = 'b' + C | 'b';
            C.Rule = 'c' + A | 'c';

            _grammar = new GrammarExpression(A, new[] { A, B, C })
                .ToGrammar();
        }

        public HiddenRightRecursionGrammar() : base(_grammar)
        {
        }
    }
}
