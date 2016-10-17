using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Tests.Common.Grammars;
using Pliant.Tests.Common;
using Pliant.Tree;

namespace Pliant.Tests.Unit.Tree
{
    [TestClass]
    public class ParseTreeEnumerableTests
    {
        [TestMethod]
        public void ParseTreeEnumeratorShouldEnumerateMultipleTrees()
        {
            var parseTester = new ParseTester(new SimpleExpressionGrammar());
            parseTester.RunParse("3+2*1+1");
            var internalForestNode = parseTester.ParseEngine.GetParseForestRootNode();
            var parseTreeEnumerable = new ParseTreeEnumerable(internalForestNode);

            var count = 0;
            foreach (var parseTree in parseTreeEnumerable)
            {
                count++;
            }
            Assert.AreEqual(4, count);
        }
    }
}
