using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Runtime;
using Pliant.Ebnf;
using System.IO;

namespace Pliant.ProtocolBuffers.Tests.Unit
{
    [TestClass]
    public class ProtocolBuffersV3GrammarTests
    {
        public TestContext TestContext { get; set; }

        const string GrammarFile = "ProtocolBufferesV3.pliant";
        const string ProtoFile = "addressbook.proto";

        [TestMethod]
        [DeploymentItem(GrammarFile, "Runtime")]
        [DeploymentItem(ProtoFile, "Runtime")]
        [Ignore]
        public void TestProtcolBuffersV3Grammar()
        {
            var testDirectory = Directory.GetCurrentDirectory();
            var ebnfPath = Path.Combine(testDirectory, "Runtime", GrammarFile);
            var ebnf = File.ReadAllText(ebnfPath);
            var ebnfGenerator = new EbnfGrammarGenerator();
            var ebnfParser = new EbnfParser();
            var ebnfDefintion = ebnfParser.Parse(ebnf);

            var parseEngine = new ParseEngine(ebnfGenerator.Generate(ebnfDefintion));

            var inputPath = Path.Combine(testDirectory, "Runtime", ProtoFile);
            var input = File.ReadAllText(inputPath);
            var parseRunner = new ParseRunner(parseEngine, input);

            while(!parseRunner.EndOfStream())
            {
                Assert.IsTrue(parseRunner.Read());
            }
            Assert.IsTrue(parseRunner.ParseEngine.IsAccepted());
        }
    }
}
