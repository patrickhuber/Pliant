using Pliant.Languages.Pdl;
using Pliant.Runtime;
using System;
using System.IO;

namespace Pliant.Samples.WithPdl
{
    class Program
    {
		public static int Main(string[] args)
		{
			// load the grammar definition file
			var path = Path.Combine(Directory.GetCurrentDirectory(), "calculator.pdl");
			var content = File.ReadAllText(path);

			// parse the grammar definition file
			var pdlParser = new PdlParser();
			var definition = pdlParser.Parse(content);

			// create the grammar, parser and scanner for our calculator language
			var grammar = new PdlGrammarGenerator().Generate(definition);
			var parser = new ParseEngine(grammar);

			var calculatorInput = "5 +30 * 2 + 1";
			var scanner = new ParseRunner(parser, calculatorInput);

			// run the scanner
			if (!scanner.RunToEnd())
			{
				Console.WriteLine($"error parsing at line {scanner.Line + 1} column {scanner.Column}");
				return -1;
			}

			// parse the calculator input
			var rootNode = parser.GetParseForestRootNode();
			Console.WriteLine(rootNode);
			// TODO: use the parse node in the calculator interpreter

			return 0;
		}
	}
}
