using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.Runtime;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class DeepTests
    {
        private static IGrammar _expressionGrammar;
        private static IGrammar _nullableGrammar;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            _expressionGrammar = GetExpressionGrammar();
            _nullableGrammar = GetNullableGrammar();
        }

        [TestMethod]
        public void DeepTest()
        {
            var deep = new Deep(_expressionGrammar);
        }

        [TestMethod]
        public void DeepTestWithNullableGrammar()
        {
            var deep = new Deep(_nullableGrammar);
        }
        
        private static IGrammar GetExpressionGrammar()
        {
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
                | T + '|' + F
                | F;
            F.Rule = '+' + F
                | '-' + F
                | 'n'
                | '(' + E + ')';

            var grammar = new GrammarExpression(S).ToGrammar();
            return grammar;
        }

        private static IGrammar GetNullableGrammar()
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

            var grammar = new GrammarExpression(SP).ToGrammar();
            return grammar;
        }
    }
}
