using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Ebnf;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tests.Unit.Runtime;
using System.Linq;

namespace Pliant.Tests.Unit.Ebnf
{
    /// <summary>
    /// Summary description for EbnfParserTests
    /// </summary>
    [TestClass]
    public class EbnfTests
    {
        private static readonly string _ebnfText = @"
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
            ~ /\w+/;
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

        private static readonly IGrammar ebnfGrammar;

        static EbnfTests()
        {
            ebnfGrammar = new EbnfGrammar();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private IParseEngine _parseEngine;

        [TestInitialize]
        public void Initialize_EbnfTests()
        {
            _parseEngine = new ParseEngine(
                ebnfGrammar, 
                new ParseEngineOptions(loggingEnabled:true)
            );
        }

        [TestMethod]
        public void EbnfShouldParseSelfDefinedGrammar()
        {
            var input = _ebnfText;
            ParseInput(input);
        }

        [TestMethod]
        public void EbnfShouldFailEmptyText()
        {
            FailParseInput("");
        }

        [TestMethod]
        public void EbnfShouldParseSingleRule()
        {
            ParseInput("Rule = Rule ;");
        }

        [TestMethod]
        public void EbnfShouldParseTwoRules()
        {
            ParseInput(@"
            Rule = Expression;
            Expression = Something;");
        }

        [TestMethod]
        public void EbnfShouldParseString()
        {
            ParseInput("Rule = \"string\";");
        }

        [TestMethod]
        public void EbnfShouldParseSetting()
        {
            ParseInput(":setting = value;");
        }

        [TestMethod]
        public void EbnfShouldParseLexerRuleFollowedBySetting()
        {
            ParseInput(@"
            Whitespace ~ /\s+/;
            :ignore = Whitespace;");
        }

        [TestMethod]
        public void EbnfShouldParseCharacter()
        {
            ParseInput("Rule = 'a';");
        }

        [TestMethod]
        public void EbnfShouldParseTrivialRegex()
        {
            ParseInput("Rule = /s/;");
        }

        [TestMethod]
        public void EbnfShouldParseRegexCharacterClass()
        {
            ParseInput("Rule = /[a-zA-Z0-9]/;");
        }

        [TestMethod]
        public void EbnfShouldParseZeroOrMore()
        {
            ParseInput(@"
            Rule = { Expression };
            Expression = Something;");
        }

        [TestMethod]
        public void EbnfShouldParseZeroOrOne()
        {
            ParseInput(@"
            Rule = [ Expression ];
            Expression = Something;");
        }

        [TestMethod]
        public void EbnfShouldParseParenthesis()
        {
            ParseInput(@"
            Rule = ( Expression );
            Expression = Something;");
        }

        [TestMethod]
        public void EbnfShouldParseAlteration()
        {
            ParseInput(@"
            Rule = Expression | Other;");
        }

        [TestMethod]
        public void EbnfShouldParseConcatenation()
        {
            ParseInput(@"
            Rule = Expression Other;");
        }

        [TestMethod]
        public void EbnfShouldParseNamespace()
        {
            ParseInput(@"
            RegularExpressions.Regex = RegularExpressions.Expression;
            RegularExpressions.Expression = RegularExpression.Term;");
        }

        [TestMethod]
        public void EbnfShouldParseEscape()
        {
            ParseInput(@"
            Regex.CharacterClassCharacter
                = /[^\]]/
                | /[\\]./;");
        }

        [TestMethod]
        public void EbnfShouldParseLexerRuleAlteration()
        {
            ParseInput(@"
            SomeLexerRule ~ 'a' | 'b'; ");
        }

        [TestMethod]
        public void EbnfShouldParseLexerRuleConcatenation()
        {
            ParseInput(@"
            SomeLexerRule ~ 'a' 'b' ;");
        }

        [TestMethod]
        public void EbnfShouldCreateCorrectParseTreeForRule()
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