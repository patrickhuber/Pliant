using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Runtime;

namespace Pliant.Tests.Unit.Languages
{
    public class LanguageBaseTest
    {
        protected IParseEngine _parseEngine;
        protected IParseRunner _parseRunner;

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

        protected void Reset()
        { 
            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "unit test is not critical code")]
        protected void FailParseAtPosition(string input, int position)
        {
            _parseRunner = new ParseRunner(_parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                if (i < position)
                    Assert.IsTrue(_parseRunner.Read(),
                        $"Line 0, Column {_parseEngine.Location} : Invalid Character {input[i]}");
                else
                    Assert.IsFalse(_parseRunner.Read(), $"parse succeeded at character position {i}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "unit test is not critical code")]
        protected void ParseInput(string input)
        {
            _parseRunner = new ParseRunner(_parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                if (!_parseRunner.Read())
                    Assert.Fail($"Line {_parseRunner.Line + 1}, Column {_parseRunner.Column + 1} : Invalid Character '{input[i]}'");
        }

        protected void Accept()
        {            
            Assert.IsTrue(_parseEngine.IsAccepted(), "input was not recognized");
        }

        protected void NotAccept()
        {
            Assert.IsFalse(_parseEngine.IsAccepted(), "input was recognized");
        }
        
        protected void NoErrors()
        { 
            
        }
    }
}
