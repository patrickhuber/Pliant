using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Tests.Common;
using Pliant.Tests.Common.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tests.Unit.Forest
{
    [TestClass]
    public class ParseGraphVisitorTests
    {

        [TestMethod]
        public void CanShowParseGraph()
        {
            var tester = new ParseTester(new SimpleExpressionGrammar());
            tester.RunParse("0+1*2+30");
            var root = tester.ParseEngine.GetParseForestRootNode();
            var visitor = new LoggingForestNodeVisitor(Console.Out);
            root.Accept(visitor);
        }
    }
}
