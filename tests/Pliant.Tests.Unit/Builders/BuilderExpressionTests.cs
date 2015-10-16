using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class BuilderExpressionTests
    {
        [TestMethod]
        public void Test_BuilderExpression_That_Can_Cast_BuilderNonTerminal_To_BuilderExpression()
        {
            ProductionBuilder term = null;
            RuleBuilder expression = null;
            expression = term;
            Assert.AreEqual(1, expression.Data.Count);
            Assert.AreEqual(1, expression.Data[0].Count);
        }

        [TestMethod]
        public void Test_BuilderExpression_That_Can_Cast_String_To_BuilderExpression()
        {
            string input = "";
            RuleBuilder expression = null;
            expression = input;
            Assert.AreEqual(1, expression.Data.Count);
            Assert.AreEqual(1, expression.Data[0].Count);
        }

        [TestMethod]
        public void Test_BuilderExpression_That_Can_Cast_String_And_String()
        {
            string input1 = "";
            string input2 = "";
            RuleBuilder expression = (_) input1 + input2;
            Assert.AreEqual(1, expression.Data.Count);
            Assert.AreEqual(2, expression.Data[0].Count);
        }

        [TestMethod]
        public void Test_BuilderExpression_That_Can_Cast_String_Or_String()
        {
            string input1 = "";
            string input2 = "";
            RuleBuilder expression = (_) input1 | input2;
            Assert.AreEqual(2, expression.Data.Count);
            Assert.AreEqual(1, expression.Data[0].Count);
            Assert.AreEqual(1, expression.Data[1].Count);
        }

        [TestMethod]
        public void Test_BuilderExpression_That_Can_Cast_Complex_Rule()
        {
            ProductionBuilder A = "A";
            RuleBuilder expression = (_)"abc" + "def" | "abc" + A;
            Assert.AreEqual(2, expression.Data.Count);
            Assert.AreEqual(2, expression.Data[0].Count);
            Assert.AreEqual(2, expression.Data[1].Count);
        }
    }
}
