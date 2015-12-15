using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ast;
using Pliant.Ebnf;
using Pliant.Grammars;

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
            _parseEngine = new ParseEngine(ebnfGrammar);
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Self_Defined_Grammar()
        {
            var input = _ebnfText;
            ParseInput(input);
        }

        [TestMethod]
        public void Test_Ebnf_That_Fails_Empty_Text()
        {
            FailParseInput("");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Single_Rule()
        {
            ParseInput("Rule = Rule ;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Two_Rules()
        {
            ParseInput(@"
            Rule = Expression;
            Expression = Something;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_String()
        {
            ParseInput("Rule = \"string\";");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Character()
        {
            ParseInput("Rule = 'a';");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Regex()
        {
            ParseInput("Rule = /[a-zA-Z0-9]/;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_ZeroOrMore()
        {
            ParseInput(@"
            Rule = { Expression };
            Expression = Something;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_OneOrZero()
        {
            ParseInput(@"
            Rule = [ Expression ];
            Expression = Something;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Parenthesis()
        {
            ParseInput(@"
            Rule = ( Expression );
            Expression = Something;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Alteration()
        {
            ParseInput(@"
            Rule = Expression | Other;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Concatenation()
        {
            ParseInput(@"
            Rule = Expression Other;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Namespace()
        {
            ParseInput(@"
            RegularExpressions.Regex = RegularExpressions.Expression;
            RegularExpressions.Expression = RegularExpression.Term;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parses_Escape()
        {
            ParseInput(@"
            Regex.CharacterClassCharacter
                = /[^\]]/
                | /[\\]./;");
        }

        [TestMethod]
        public void Test_Ebnf_That_Parse_Tree_For_Rule_Is_Created_Correctly()
        {
            var node = ParseInput(@"
            SomeRule = 'a' 'b' 'c' ;
            ") as ISymbolNode;
            Assert.IsNotNull(node);

            var visitor = new LoggingNodeVisitor();
            node.Accept(visitor);

            var log = visitor.VisitLog;
            Assert.IsTrue(log.Count > 0);
        }

        private INode ParseInput(string input)
        {
            var parseInterface = new ParseInterface(_parseEngine, input);
            for (int i = 0; i < input.Length; i++)
            {
                Assert.IsTrue(parseInterface.Read(), "Error found in position {0}", parseInterface.Position);
            }
            Assert.IsTrue(parseInterface.ParseEngine.IsAccepted());
            return parseInterface.ParseEngine.GetParseForestRoot();
        }

        private void FailParseInput(string input)
        {
            var parseInterface = new ParseInterface(_parseEngine, input);
            Assert.IsFalse(parseInterface.ParseEngine.IsAccepted());
        }
    }
}