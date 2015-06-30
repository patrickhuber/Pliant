using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Bnf;
using System.IO;
using System.Text;

namespace Pliant.Tests.Unit.Bnf
{
    /// <summary>
    ///     <see cref="http://cui.unige.ch/db-research/Enseignement/analyseinfo/AboutBNF.html"/>
    /// </summary>
    [TestClass]
    public class BnfTests
    {
        public TestContext TestContext { get; set; }
        
        private string _bnfText = @"
            <syntax>         ::= <rule> | <rule> <syntax>
            <rule>           ::= <identifier> ""::="" <expression> <line-end>
            <expression>     ::= <list> | <list> ""|"" <expression>
            <line-end>       ::= <EOL> | <line-end> <line-end>
            <list>           ::= <term > | <term> <list>
            <term>           ::= <literal > | <identifier>
            <identifier>     ::= ""<"" <rule-name> "">""
            <literal>        ::= '""' <text> '""' | ""'"" <text> ""'""";

        [TestMethod]
        public void Test_Bnf_That_String_Iterate_Sets_Baseline()
        {
            var stringReader = new StringReader(_bnfText);
            int c = 0;
            while ((c = stringReader.Read()) != -1) ;                
        }

        [TestMethod]
        public void Test_Bnf_That_Parse_Produces_Bnf_Chart()
        {
            var grammar = new BnfGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, _bnfText);
            var stringReader = new StringReader(_bnfText);
            
            while (!parseInterface.EndOfStream())
            {                 
                if (!parseInterface.Read())
                {
                    var position = parseInterface.Position;
                    var startIndex = 0;
                    for (int i = position; i >= 0; i--)
                    {
                        if (_bnfText[i] == '\n' && i > 0)
                            if (_bnfText[i - 1] == '\r')
                            {
                                startIndex = i;
                                break;
                            }
                    }
                    var endIndex = _bnfText.IndexOf("\r\n", position);
                    endIndex = endIndex < 0 ? _bnfText.Length : endIndex;
                    var length = endIndex - startIndex;
                    var stringBuilder = new StringBuilder();
                    stringBuilder
                        .AppendFormat("Error parsing input string at position {0}.", parseInterface.Position)
                        .AppendLine()
                        .AppendFormat("start: {0}", startIndex)
                        .AppendLine()
                        .AppendLine(_bnfText.Substring(startIndex, length));

                    Assert.Fail(stringBuilder.ToString());
                }
            }
        }
    }
}
