using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;

namespace Pliant.Tests.Unit.Builders.Expressions
{
    [TestClass]
    public class ProductionExpressionTests
    {
        [TestMethod]
        public void ProductionExpressionShouldSupportStringLexerRules()
        {
            ProductionExpression S = "S", B="B", C="C", D="D";
            S.Rule = S & D;
            B.Rule = "abc" & C;
            //C.Rule = "abc" | "def";

            Console.WriteLine("this is a break");
        }
    }
}
