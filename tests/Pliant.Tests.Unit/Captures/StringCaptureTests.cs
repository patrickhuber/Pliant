using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Captures;

namespace Pliant.Tests.Unit.Captures
{
    [TestClass]
    public class StringCaptureTests
    {
        [TestMethod]
        public void EqualShouldWorkWithStrings()
        {
            var expected = "test";
            var capture = expected.AsCapture();
            Assert.IsTrue(capture.Equals(expected));
        }
    }
}
