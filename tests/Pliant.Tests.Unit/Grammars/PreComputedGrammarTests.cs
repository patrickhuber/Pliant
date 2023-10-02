using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Collections;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.Languages.Regex;
using Pliant.Tests.Common.Grammars;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class PreComputedGrammarTests
    {
        private static IGrammar _jsonGrammar;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            _jsonGrammar = GetJsonGrammar();
        }

        [TestMethod]
        public void PreComputedGrammarShouldLoadExpressionGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(new ExpressionGrammar());
            
        }

        [TestMethod]
        public void PreComputedGrammarShouldLoadNullableGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(new NullableGrammar());
        }        

        [TestMethod]
        public void PreComputedGrammarShouldLoadJsonGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(_jsonGrammar);
        }

        [TestMethod]
        public void PreComputedGrammarIsRightRecursiveShouldFindSimpleRecursion()
        {
            var preComputedGrammar = new PreComputedGrammar(new RightRecursionGrammar());            
        }

        [TestMethod]
        public void PreComputedGrammarIsRightRecursiveShouldFindCyclicRecursion()
        {
            var grammar = new HiddenRightRecursionGrammar();
            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                if (production.IsEmpty)
                    continue;
                if (production.RightHandSide[production.RightHandSide.Count - 1].SymbolType != SymbolType.NonTerminal)
                    continue;
                Assert.IsTrue(grammar.IsRightRecursive(production), $"expected {production} to be right recursive");
            }
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
                new[] { new WhitespaceLexerRule() },
                null).ToGrammar();
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
