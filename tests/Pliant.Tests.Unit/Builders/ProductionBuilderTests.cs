using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class ProductionBuilderTests
    {
        [TestMethod]
        public void Test_ProductionBuilder_That_Can_Implicit_Cast_Rule()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");
            s.Rule(a);
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Can_Implicit_Cast_String()
        {
            var s = new ProductionBuilder("S");
            s.Rule("string");
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Can_Implicit_Cast_Char()
        {
            var s = new ProductionBuilder("S");
            s.Rule('c');
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Given_Two_ProductionBuilders_Can_Resolve_Alteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");
            var b = new ProductionBuilder("B");

            s.Rule(a)
             .Or(b);
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Given_Two_ProductionBuilders_Can_Resolve_Concatenation()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");
            var b = new ProductionBuilder("B");

            s.Rule(a, b);
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Given_Char_And_ProductionBuilder_Resolves_Alteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule('c')
             .Or(a);
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Given_ProductionBuilder_And_Char_Resolves_Alteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule(a)
             .Or('c');
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Given_String_And_ProductionBuilder_Resolves_Alteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule("string")
                .Or(a);
        }

        [TestMethod]
        public void Test_ProductionBuilder_That_Given_ProductionBuilder_And_String_Resolves_Alteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule(a)
                .Or("string");
        }
        
        [TestMethod]
        public void Test_ProductionBuilder_That_Given_Two_Strings_Resolves_Alteration()
        {
            var s = new ProductionBuilder("S");
            var a = new ProductionBuilder("A");

            s.Rule("one")
                .Or("two");
            var grammarBuilder = new GrammarBuilder(s, new[] { s, a });
        }        
    }
}

