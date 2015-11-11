using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;

namespace Pliant.Tests.Unit.RegularExpressions
{
    /// <summary>
    /// Summary description for RegexParserTests
    /// </summary>
    [TestClass]
    public class RegexParserTests
    {
        public TestContext TestContext { get; set; }
        
        [TestMethod]
        public void Test_RegexParser_That_Single_Character_Returns_Proper_Object()
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse("a");
            Assert.IsNotNull(regex);
        }
    }
}
