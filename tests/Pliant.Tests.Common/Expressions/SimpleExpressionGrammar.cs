using Pliant.Builders.Expressions;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Expressions
{
    public class SimpleExpressionGrammar : GrammarWrapper
    {
        private static readonly IGrammar _innerGrammar;

        static SimpleExpressionGrammar()
        {
            var digit = new DigitTerminal();
            ProductionExpression E = "E";
            E.Rule =
                E + "+" + E
                | E + "*" + E
                | digit;
            _innerGrammar = new GrammarExpression(E, new[] { E }).ToGrammar();
        }

        public SimpleExpressionGrammar()
            : base(_innerGrammar)
        { }
    }
}
