# Pliant
Implementation of a modified Earley parser in C# inspired by the Marpa Parser project.

## Description
Pliant is a table driven parser that implements the Earley algorithm. Two optimizations are added to handle issues with the original Earley implementation: 

1. Optimization from Joop Leo to efficiently handle right recursions. 
2. Bug fix from Aycock and Horspool to handle nullable predictions

## Using the Code

### Creating Grammars

#### Using the Grammar Builder classes

```CSharp
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Automata;

public static int Main(string[] args)
{
	var digits = CreateDigitLexerRule();
	var whitespace = CreateWhitespaceLexerRule();
	
	ProductionBuilder 
		Calculator = "Calculator",
		Factor = "Factor",
		Term = "Term",
		Expression = "Expression",
		Number = "Number";
		
	Calculator.Definition 
		= Expression ;
		
	Expression.Definition
		= Expression + '+' + Term 
		| Term ;

	Term.Definition 
		= Term + '*' + Factor
		| Factor ;
		
	Factor.Definition 
		= Number ;
		
	Number.Definition
		= digits;
		
	var grammar = new GrammarBuilder(
		Calculator, 
		new[]{ Calculator, Factor, Term, Expression, Number }, 
		new []{ whitespace })
	.ToGrammar();	
	
	// TODO: Use the grammar in a parse.
}

private static BaseLexerRule CreateDigitLexerRule()
{
	var startState = new DfaState();
	var finalState = new DfaState(true);
	var digitTransition = new DfaTransition(new DigitTerminal(), finalState);
	startState.AddTransition(digitTransition);
	finalState.AddTransition(digitTransition);
	return new DfaLexerRule(startState, "[\\d]+");
}

private static ILexerRule CreateWhitespaceLexerRule()
{
	var whitespaceTerminal = new WhitespaceTerminal();
	var startState = new DfaState();
	var finalState = new DfaState(true);
	var whitespaceTransition = new DfaTransition(whitespaceTerminal, finalState);
	startState.AddTransition(whitespaceTransition);
	finalState.AddTransition(whitespaceTransition);
	return new DfaLexerRule(startState, new TokenType("[\\s]+"));	
}		
```

#### Using the Ebnf Text Interface ( *work in progress* )

```CSharp
public static int Main (string[] args)
{
	var grammarText = @"
	Calculator 
		= Expression;
		
	Expression 
		= Expression Term
		| Term;
		
	Term 
		= Term '*' Factor
		| Factor;
		
	Factor 
		= Number ;
	
	Number 
		= Digits;
		
	Digits ~ /[0-9]+/ ;
	Whitespace ~ /[\\s]+/ ;
	
	:start = Calculator;
	:ignore = Whitespace;";
	
	var compiler = new EbnfCompiler();
	var grammar = compiler.Compile(grammarText);
	
	// TODO: use the grammar in a parse.
}
```

### Using Grammars to Recognize Input

Using the calculator grammar from above, we can parse input by constructing
a parse engine and parse interface instance.

```csharp
var input = "(1 + 1) * 3 + 2";

// use the calculator grammar from above
var parseEngine = new ParseEngine(grammar);

// use the parse interface to querying the parse engine
// for state and use that state to select lexer rules.
var parseInterface = new ParseInterface(parseEngine, input);

// when a parse is recognized, the parse engine is allowed to move
// forward and continues to accept symbols. 
var recognzied = false;
var errorPosition = 0;
while(!parseInterface.EndOfStream())
{
	recognzied = parseInterface.Read();
	if(!recognized)
	{	
		errorPosition = parseInterface.Position;
		break;
	}
}

// For a parse to be accepted, all parse rules are completed and a trace
// has been made back to the parse root.
// A parse must be recognized in order for acceptance to have meaning.
var accepted = false;
if(recognized)
{
	accepted = parseInterface.IsAccepted();
	if(!accepted)
		errorPosition = parseInterface.Position;
}
Console.WriteLine($"Recognized: {recognized}, Accepted: {accepted}");
if(!recognized || !accepted)
	Console.Error.WriteLine($"Error at position {errorPosition}");
```

## References

* [berkeley earley cs164](http://inst.eecs.berkeley.edu/~cs164/fa10/earley/earley.html)
* [washington edu ling571](http://courses.washington.edu/ling571/ling571_fall_2010/slides/parsing_earley.pdf)
* [unt cse earley](http://www.cse.unt.edu/~tarau/teaching/NLP/Earley%20parser.pdf)
* [wikipedia](http://en.wikipedia.org/wiki/Earley_parser)
* [optimizing right recursion](http://loup-vaillant.fr/tutorials/earley-parsing/right-recursion)
* [marpa parser](http://jeffreykegler.github.io/Ocean-of-Awareness-blog/)
* [joop leo - optimizing right recursions](http://www.sciencedirect.com/science/article/pii/030439759190180A)
* [parsing techniques - a practical guide](http://amzn.com/B0017AMLL8)
* [practical earley parsing](http://webhome.cs.uvic.ca/~nigelh/Publications/PracticalEarleyParsing.pdf)
* [detailed description of leo optimizations and internals of marpa](https://github.com/jeffreykegler/kollos/blob/master/notes/misc/leo2.md)
* [theory of Marpa Algorithm](https://docs.google.com/file/d/0B9_mR_M2zOc4Ni1zSW5IYzk3TGc/edit)
* [parse tree forest creation](http://www.sciencedirect.com/science/article/pii/S1571066108001497)
* [cs theory stackexchange, leo optimization parse tree creation](http://cstheory.stackexchange.com/q/31182/32787)
* [insights on lexer creation](https://youtu.be/XaScLywH2CI)
* [incremental reparsing](http://www.aclweb.org/anthology/E89-1033.pdf)
* [An extension of Earley's Algorithm for extended grammars](http://link.springer.com/chapter/10.1007%2F978-1-4020-3953-9_22)