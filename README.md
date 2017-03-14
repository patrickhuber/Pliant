# Pliant
Implementation of a modified Earley parser in C# inspired by the Marpa Parser project.

## Build Status
![](https://patrickhuber.visualstudio.com/_apis/public/build/definitions/d758f02e-3764-4572-aaff-9378f05b48f9/3/badge)

## Gitter Chat
[![Gitter](https://img.shields.io/gitter/room/pliant.net/lobby.svg?maxAge=2592000)](https://gitter.im/Pliant-net/Lobby)

## Description
Pliant is a table driven parser that implements the Earley algorithm. Two optimizations are added to handle issues with the original Earley implementation: 

1. Optimization from Joop Leo to efficiently handle right recursions. 
2. Bug fix from Aycock and Horspool to handle nullable predictions

## Using the Code

### Getting Started

Add a reference to the Pliant libarary using [NuGet](http://www.nuget.org/packages/Pliant/)

```
PM> Install-Package Pliant
```

### Creating Grammars

#### Using the Grammar Expression classes

```CSharp
using Pliant.Builders;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Automata;

public static int Main(string[] args)
{
	var digits = CreateDigitLexerRule();
	var whitespace = CreateWhitespaceLexerRule();
	
	ProductionExpression
		Calculator = "Calculator",
		Factor = "Factor",
		Term = "Term",
		Expression = "Expression",
		Number = "Number";
		
	Calculator.Rule 
		= Expression ;
		
	Expression.Rule
		= Expression + '+' + Term 
		| Term ;

	Term.Rule 
		= Term + '*' + Factor
		| Factor ;
		
	Factor.Rule 
		= Number ;
		
	Number.Rule
		= digits;
		
	var grammar = new GrammarExpression(
		Calculator, 
		new []{ Calculator, Factor, Term, Expression, Number }, 
		new []{ new LexerRuleModel(whitespace) })
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
		= Expression '+' Term
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
	
	var definition = new EbnfParser().Parse(grammarText);
	var grammar = new EbnfGrammarGenerator().Generate(definition);
	
	// TODO: use the grammar in a parse.
}
```
## Recognizing and Parse Trees

### Using ParseEngine, ParseRunner and Grammars to Recognize Input

Using the calculator grammar from above, we can parse input by constructing
a parse engine and parse runner instance.

```csharp
var input = "1 + 1 * 3 + 2";

// use the calculator grammar from above
var parseEngine = new ParseEngine(grammar);

// use the parse runner to query the parse engine for state
// and use that state to select lexer rules.
var parseRunner = new ParseRunner(parseEngine, input);

// when a parse is recognized, the parse engine is allowed to move
// forward and continues to accept symbols. 
var recognized = false;
var errorPosition = 0;
while(!parseRunner.EndOfStream())
{
	recognized = parseRunner.Read();
	if(!recognized)
	{	
		errorPosition = parseRunner.Position;
		break;
	}
}

// For a parse to be accepted, all parse rules are completed and a trace
// has been made back to the parse root.
// A parse must be recognized in order for acceptance to have meaning.
var accepted = false;
if(recognized)
{
	accepted = parseRunner.ParseEngine.IsAccepted();
	if(!accepted)
		errorPosition = parseRunner.Position;
}
Console.WriteLine($"Recognized: {recognized}, Accepted: {accepted}");
if(!recognized || !accepted)
	Console.Error.WriteLine($"Error at position {errorPosition}");
```

### Using ParseEngine, ParseRunner, Forest API and Grammars to build a parse tree.

The process for creating a parse tree is the same as recognizing input. 
In fact, when running the ParseEngine, a Sparsley Packed Parse Forest (SPPF) is created 
in the background. The parse forest is presented in a specialized format to promote density and allow for 
computational complexity similar to that of running the recognizer alone. 

The easiest way to use the parse forest is use a internal node tree visitor on the parse forest root 
with a SelectFirstChildDisambiguationAlgorithm instance controling disambiguation of thet forest.

If the parse is ambiguous, you may want to supply a custom IForestDisambiguationAlgorithm.

```csharp
// get the parse forest root from the parse engine
var parseForestRoot = parseEngine.GetParseForestRootNode();

// create a internal tree node and supply the disambiguation algorithm for tree traversal.
var parseTree = new InternalTreeNode(
    parseForestRoot,
    new SelectFirstChildDisambiguationAlgorithm());
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
* [practical earley parsing](http://www.cs.uvic.ca/~nigelh/Publications/PracticalEarleyParsing.pdf)
* [detailed description of leo optimizations and internals of marpa](https://github.com/jeffreykegler/kollos/blob/master/notes/misc/leo2.md)
* [theory of Marpa Algorithm](https://docs.google.com/file/d/0B9_mR_M2zOc4Ni1zSW5IYzk3TGc/edit)
* [parse tree forest creation](http://www.sciencedirect.com/science/article/pii/S1571066108001497)
* [cs theory stackexchange, leo optimization parse tree creation](http://cstheory.stackexchange.com/q/31182/32787)
* [insights on lexer creation](https://youtu.be/XaScLywH2CI)
* [incremental reparsing](http://www.aclweb.org/anthology/E89-1033.pdf)
* [An extension of Earley's Algorithm for extended grammars](http://link.springer.com/chapter/10.1007%2F978-1-4020-3953-9_22)
* [Finding nullable productions in a grammar](http://cstheory.stackexchange.com/a/2493/32787)
* [Directly-Executable Earley Parsing (Aycock and Horspool, 2001)](http://link.springer.com/chapter/10.1007%2F3-540-45306-7_16#)
* [Context Senitive Earley Parsing](https://web.archive.org/web/20110708224600/https://danielmattosroberts.com/earley/context-sensitive-earley.pdf)
