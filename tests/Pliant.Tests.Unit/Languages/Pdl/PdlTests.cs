using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Languages.Pdl;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tests.Unit.Runtime;

namespace Pliant.Tests.Unit.Languages.Pdl
{
    /// <summary>
    /// Summary description for PdlParserTests
    /// </summary>
    [TestClass]
    public class PdlTests
    {
        private static readonly string _pdlText = @"
        Grammar
            = { Rule } ;
        Rule
            = RuleName '=' Expression ';' ;
        RuleName
            = Identifier ;
        Expression
            = Identifier
            | Terminal
            | Optional
            | Repetition
            | Grouping
            | Expression '|' Expression
            | Expression Expression ;
        Optional
            = '[' Expression ']';
        Repetition
            = '{' Expression '}';
        Grouping
            = '(' Expression ')';
        Identifier
            = Letter { Letter | Digit | '_' };
        Terminal
            = '""' String '""'
            | ""'"" Character ""'""
            | '/' Regex '/' ;
        String
            = { StringCharacter };
        StringCharacter
            = /[^\""]/
            | '\\' AnyCharacter ;
        Character
            = SingleCharacter
            | '\\' SimpleEscape ;
        SingleCharacter
            ~ /[^']/;
        SimpleEscape
            ~ ""'"" | '""' | '\\' | '0' | 'a' | 'b'
            | 'f' | 'n'  | 'r' | 't' | 'v' ;
        Digit
            ~ /[0-9]/ ;
        Letter
            ~ /[a-zA-Z]/;
        Whitespace
            ~ /\s+/;
        Regex
            = ['^'] Regex.Expression ['$'] ;
        Regex.Expression
            = [Regex.Term]
            | Regex.Term '|' Regex.Expression ;
        Regex.Term
            = Regex.Factor [Regex.Term] ;
        Regex.Factor
            = Regex.Atom [Regex.Iterator] ;
        Regex.Atom
            = '.'
            | Regex.Character
            | '(' Regex.Expression ')'
            | Regex.Set ;
        Regex.Set
            = Regex.PositiveSet
            | Regex.NegativeSet ;
        Regex.PositiveSet
            = '[' Regex.CharacterClass ']'
            | ""[^"" Regex.CharacterClass ""]"" ;
        Regex.CharacterClass
            = Regex.CharacterRange { Regex.CharacterRange } ;
        Regex.CharacterRange
            = Regex.CharacterClassCharacter
            | Regex.CharacterClassCharacter '-' Regex.CharacterClassCharacter;
        Regex.CharacterClassCharacter
            ~ /[^\]]/
            | '\\' /./ ;
        Regex.Character
            ~ /[^.^$()[\] + *?\\]/
            | '\\' /./ ;
        :ignore
            = Whitespace;";

        private static readonly IGrammar pdlGrammar;

        static PdlTests()
        {
            pdlGrammar = new PdlGrammar();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private IParseEngine _parseEngine;

        [TestInitialize]
        public void Initialize_PdlTests()
        {
            _parseEngine = new ParseEngine(
                pdlGrammar, 
                new ParseEngineOptions(loggingEnabled:true)
            );
        }

        [TestMethod]
        public void PdlShouldParseSelfDefinedGrammar()
        {
            var input = _pdlText;
            ParseInput(input);
        }

        [TestMethod]
        public void PdlShouldFailEmptyText()
        {
            FailParseInput("");
        }

        [TestMethod]
        public void PdlShouldParseSingleRule()
        {
            ParseInput("Rule = Rule ;");
        }

        [TestMethod]
        public void PdlShouldParseTwoRules()
        {
            ParseInput(@"
            Rule = Expression;
            Expression = Something;");
        }

        [TestMethod]
        public void PdlShouldParseString()
        {
            ParseInput("Rule = \"string\";");
        }

        [TestMethod]
        public void PdlShouldParseSetting()
        {
            ParseInput(":setting = value;");
        }

        [TestMethod]
        public void PdlShouldParseLexerRuleFollowedBySetting()
        {
            ParseInput(@"
            Whitespace ~ /\s+/;
            :ignore = Whitespace;");
        }

        [TestMethod]
        public void PdlShouldParseCharacter()
        {
            ParseInput("Rule = 'a';");
        }

        [TestMethod]
        public void PdlShouldParseTrivialRegex()
        {
            ParseInput("Rule = /s/;");
        }

        [TestMethod]
        public void PdlShouldParseRegexCharacterClass()
        {
            ParseInput("Rule = /[a-zA-Z0-9]/;");
        }

        [TestMethod]
        public void PdlShouldParseZeroOrMore()
        {
            ParseInput(@"
            Rule = { Expression };
            Expression = Something;");
        }

        [TestMethod]
        public void PdlShouldParseZeroOrOne()
        {
            ParseInput(@"
            Rule = [ Expression ];
            Expression = Something;");
        }

        [TestMethod]
        public void PdlShouldParseParenthesis()
        {
            ParseInput(@"
            Rule = ( Expression );
            Expression = Something;");
        }

        [TestMethod]
        public void PdlShouldParseAlteration()
        {
            ParseInput(@"
            Rule = Expression | Other;");
        }

        [TestMethod]
        public void PdlShouldParseConcatenation()
        {
            ParseInput(@"
            Rule = Expression Other;");
        }

        [TestMethod]
        public void PdlShouldParseNamespace()
        {
            ParseInput(@"
            RegularExpressions.Regex = RegularExpressions.Expression;
            RegularExpressions.Expression = RegularExpression.Term;");
        }

        [TestMethod]
        public void PdlShouldParseEscape()
        {
            ParseInput(@"
            Regex.CharacterClassCharacter
                = /[^\]]/
                | /[\\]./;");
        }


        [TestMethod]
        [Ignore]
        public void PdlShouldParseNullRule()
        {
            // Still need to determine the syntax for this or if it should be supported
            ParseInput(@"
            Rule = ;");
        }

        [TestMethod]
        public void PdlShouldParseLexerRuleAlteration()
        {
            ParseInput(@"
            SomeLexerRule ~ 'a' | 'b'; ");
        }

        [TestMethod]
        public void PdlShouldParseLexerRuleConcatenation()
        {
            ParseInput(@"
            SomeLexerRule ~ 'a' 'b' ;");
        }

        [TestMethod]
        public void PdlShouldCreateCorrectParseTreeForRule()
        {
            var node = ParseInput(@"
            SomeRule = 'a' 'b' 'c' ;
            ") as ISymbolForestNode;
            Assert.IsNotNull(node);
            
            var visitor = new LoggingNodeVisitor(
                new SelectFirstChildDisambiguationAlgorithm());
            node.Accept(visitor);

            var log = visitor.VisitLog;
            Assert.IsTrue(log.Count > 0);
        }

        private IForestNode ParseInput(string input)
        {
            var parseRunner = new ParseRunner(_parseEngine, input);
            while (!parseRunner.EndOfStream())
                if(!parseRunner.Read())
                    Assert.Fail($"Error found in on line {parseRunner.Line}, column {parseRunner.Column}");
            
            if(!parseRunner.ParseEngine.IsAccepted())
                Assert.Fail("Parse was not accepted.");

            return parseRunner.ParseEngine.GetParseForestRootNode();
        }

        private void FailParseInput(string input)
        {
            var parseRunner = new ParseRunner(_parseEngine, input);
            Assert.IsFalse(parseRunner.ParseEngine.IsAccepted());
        }
    }
}