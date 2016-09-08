using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class PreComputedGrammarTests
    {
        private static IGrammar _expressionGrammar;
        private static IGrammar _nullableGrammar;
        private static IGrammar _jsonGrammar;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            _expressionGrammar = GetExpressionGrammar();
            _nullableGrammar = GetNullableGrammar();
            _jsonGrammar = GetJsonGrammar();
        }

        [TestMethod]
        public void PreComputedGrammarTest()
        {
            var preComputedGrammar = new PreComputedGrammar(_expressionGrammar);
        }

        [TestMethod]
        public void PreComputedGrammarTestWithNullableGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(_nullableGrammar);
        }        

        [TestMethod]
        public void PreComputedGrammarTestWithJsonGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(_jsonGrammar);
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

        private static IGrammar GetJsonGrammar()
        {
            ProductionExpression
                Json = "Json",
                Object = "Object",
                Pair = "Pair",
                PairRepeat = "PairRepeat",
                Array = "Array",
                Value = "Value",
                ValueRepeat = "ValueRepeat";

            var number = new NumberLexerRule();
            var @string = String();

            Json.Rule =
                Value;

            Object.Rule =
                '{' + PairRepeat + '}';

            PairRepeat.Rule =
                Pair
                | Pair + ',' + PairRepeat
                | (Expr)null;

            Pair.Rule =
                (Expr)@string + ':' + Value;

            Array.Rule =
                '[' + ValueRepeat + ']';

            ValueRepeat.Rule =
                Value
                | Value + ',' + ValueRepeat
                | (Expr)null;

            Value.Rule = (Expr)
                @string
                | number
                | Object
                | Array
                | "true"
                | "false"
                | "null";

            return new GrammarExpression(
                Json,
                null,
                new[] { new WhitespaceLexerRule() }).ToGrammar();
        }

        private static BaseLexerRule String()
        {
            // ["][^"]+["]
            const string pattern = "[\"][^\"]+[\"]";
            return CreateRegexDfa(pattern);
        }


        private static BaseLexerRule CreateRegexDfa(string pattern)
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse(pattern);
            var regexCompiler = new RegexCompiler();
            var dfa = regexCompiler.Compile(regex);
            return new DfaLexerRule(dfa, pattern);
        }

    }
}
