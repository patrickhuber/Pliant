using Pliant.Bnf;
using Pliant.Builders;
using Pliant.Dfa;
using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.IO;
using System.Text;

namespace Pliant.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sampleBnf = @"
            <syntax>         ::= <rule> | <rule> <syntax>
            <rule>           ::= <identifier> ""::="" <expression> <line-end>
            <expression>     ::= <list> | <list> ""|"" <expression>
            <line-end>       ::= <EOL> | <line-end> <line-end>
            <list>           ::= <term > | <term> <list>
            <term>           ::= <literal > | <identifier>
            <identifier>     ::= ""<"" <rule-name> "">""
            <literal>        ::= '""' <text> '""' | ""'"" <text> ""'""";

            var grammar = new BnfGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, sampleBnf);
            var stringReader = new StringReader(sampleBnf);

            while (!parseInterface.EndOfStream() && parseInterface.Read()) { }

            var result = parseInterface.ParseEngine.IsAccepted();
            Console.Write(result);
        }
    }
}
