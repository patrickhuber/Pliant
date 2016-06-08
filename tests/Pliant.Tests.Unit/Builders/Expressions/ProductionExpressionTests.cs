using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;

namespace Pliant.Tests.Unit.Builders.Expressions
{
    [TestClass]
    public class ProductionExpressionTests
    {
        [TestMethod]
        public void ProductionExpressionShouldSupportIsolatedStringLexerRule()
        {
            ProductionExpression
                S = "S";
            S.Rule = "abc";

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfStringLexerRuleAndNonTerminal()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = "s" + A;
            A.Rule = "a";

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfTwoStringLexerRules()
        {
            ProductionExpression
                S = "S";
            S.Rule = (Expr)"s" + "d";

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfNonTerminalAndStringLexerRule()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = A + "s";
            A.Rule = "a";
            
            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfStringLexerRuleAndNonTerminal()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = "s" | A;
            A.Rule = "a";

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[1].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfTwoStringLexerRules()
        {
            ProductionExpression
                S = "S";
            S.Rule = (Expr)"s" | "d";

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[1].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfNonTerminalAndStringLexerRule()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = A | "s";
            A.Rule = "a";

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[1].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportIsolatedTerminalLexerRule()
        {
            ProductionExpression
                S = "S";
            S.Rule = 's';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfTerminalLexerRuleAndNonTerminal()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = 's' + A;
            A.Rule = 'a';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfTwoTerminalLexerRules()
        {
            ProductionExpression
                S = "S";
            S.Rule = (Expr)'s' + 'd';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfNonTerminalAndTerminalLexerRule()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = A + 's';
            A.Rule = 'a';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfNonTerminalAndTerminalLexerRule()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = A | 's';
            A.Rule = 'a';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfTwoTerminalLexerRules()
        {
            ProductionExpression
                S = "S";
            S.Rule = (Expr)'s' | 'd';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[1].Symbols.Count);
        }


        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfTerminalLexerRuleAndNonTerminal()
        {
            ProductionExpression
                S = "S",
                A = "A";
            S.Rule = 's' | A;
            A.Rule = 'a';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[1].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportConcatenationOfTwoNonTerminals()
        {
            ProductionExpression
                S = "S",
                A = "A",
                B = "B";

            S.Rule = A + B;
            A.Rule = 'a';
            B.Rule = 'b';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(1, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(2, S.ProductionModel.Alterations[0].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);
            
            // Test B
            Assert.IsNotNull(B.ProductionModel);
            Assert.AreEqual(1, B.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, B.ProductionModel.Alterations[0].Symbols.Count);
        }

        [TestMethod]
        public void ProductionExpressionShouldSupportAlterationOfTwoNonTerminals()
        {
            ProductionExpression
                S = "S",
                A = "A",
                B = "B";

            S.Rule = A | B;
            A.Rule = 'a';
            B.Rule = 'b';

            // Test S
            Assert.IsNotNull(S.ProductionModel);
            Assert.AreEqual(2, S.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[0].Symbols.Count);
            Assert.AreEqual(1, S.ProductionModel.Alterations[1].Symbols.Count);

            // Test A
            Assert.IsNotNull(A.ProductionModel);
            Assert.AreEqual(1, A.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, A.ProductionModel.Alterations[0].Symbols.Count);

            // Test B
            Assert.IsNotNull(B.ProductionModel);
            Assert.AreEqual(1, B.ProductionModel.Alterations.Count);
            Assert.AreEqual(1, B.ProductionModel.Alterations[0].Symbols.Count);
        }
    }
}
