using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Captures;
using System.Text;

namespace Pliant.Tests.Unit.Captures
{
    [TestClass]
    public class StringBuilderCaptureTests
    {
        [TestMethod]
        public void EqualShouldWorkWithStrings()
        {
            var expected = new StringBuilder("test");
            var capture = expected.AsCapture();
            Assert.IsTrue(capture.Equals(expected));
        }
    }
}
