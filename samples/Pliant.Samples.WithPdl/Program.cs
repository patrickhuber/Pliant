using Pliant.Languages.Pdl;
using Pliant.Runtime;
using Pliant.Samples.WithPdl.Ast;
using Pliant.Tree;
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
			Console.WriteLine(calculatorInput);
			var scanner = new ParseRunner(parser, calculatorInput);

			// run the scanner
			if (!scanner.RunToEnd())
			{
				Console.WriteLine($"error parsing at line {scanner.Line + 1} column {scanner.Column}");
				return -1;
			}

			// parse the calculator input
			var rootNode = parser.GetParseForestRootNode();
			
			// get the ast
			var visitor = new Visitor();
			var tree = new InternalTreeNode(rootNode);
			tree.Accept(visitor);

			// interpret the result
			var result = Interpret(visitor.Calculator);					
			Console.WriteLine(result);
			return 0;
		}

		public static uint Interpret(Calculator calculator)
		{
			return Interpret(calculator.Expression);
		}

		public static uint Interpret(Expression expression)
		{ 
			if(expression is ExpressionOperatorTerm expressionOperatorTerm) 
			{
				var lhs = Interpret(expressionOperatorTerm.Expression);
				var rhs = Interpret(expressionOperatorTerm.Term);
				var op = expressionOperatorTerm.Operator;
				switch (op)
				{
					case Operator.Plus:
						return lhs + rhs;
					case Operator.Minus:
						return lhs - rhs;
				}
				throw new InvalidOperationException("Unable to process operator {op} in expression. Expected Plus or Minus.");
			}
			return Interpret(expression.Term);
		}

		public static uint Interpret(Term term)
		{ 
			if (term is TermOperatorFactor termOperatorFactor) 
			{
				var lhs = Interpret(termOperatorFactor.Term);
				var rhs = Interpret(termOperatorFactor.Factor);
				var op = termOperatorFactor.Operator;
                switch (op) 
				{
					case Operator.Multiply:
						return lhs * rhs;
					case Operator.Divide:
						return lhs / rhs;
				}
			}
			return Interpret(term.Factor);
		}

		public static uint Interpret(Factor factor)
		{
			return factor.Number;
		}
	}
}
