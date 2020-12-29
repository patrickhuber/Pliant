using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Runtime;

namespace Pliant.Tests.Unit.Languages
{
    public class LanguageBaseTest
    {
        protected IParseEngine _parseEngine;

        protected void Initialize(IGrammar grammar, ParseEngineOptions options = null)
        {
            if (options is null)
                options = new ParseEngineOptions(optimizeRightRecursion: true, loggingEnabled: true);
            _parseEngine = new ParseEngine(grammar, options);
        }

        protected void ParseAndAcceptInput(string input)
        {
            ParseInput(input);
            Accept();
        }

        protected void ParseAndNotAcceptInput(string input)
        {
            ParseInput(input);
            NotAccept();
        }

        protected void FailParseAtPosition(string input, int position)
        {
            var parseRunner = new ParseRunner(_parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                if (i < position)
                    Assert.IsTrue(parseRunner.Read(),
                        $"Line 0, Column {_parseEngine.Location} : Invalid Character {input[i]}");
                else
                    Assert.IsFalse(parseRunner.Read());
        }

        protected void ParseInput(string input)
        {
            var parseRunner = new ParseRunner(_parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                if (!parseRunner.Read())
                    Assert.Fail($"Line {parseRunner.Line + 1}, Column {parseRunner.Column + 1} : Invalid Character '{input[i]}'");
        }

        protected void Accept()
        {
            Assert.IsTrue(_parseEngine.IsAccepted(), "input was not recognized");
        }

        protected void NotAccept()
        {
            Assert.IsFalse(_parseEngine.IsAccepted(), "input was recognized");
        }
    }
}
