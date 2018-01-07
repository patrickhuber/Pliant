# Grammar Syntax Documentation

Grammars in Pliant are defined by an bnf dialect similar to abnf and ebnf. Some additional control statements have been added for ignoring whitespace and specifying start productions.

## Grammar of Grammars

The supported language for grammars in Pliant is defined by the following BNF grammar. 

```ebnf
definition =   
      block 
    | block, definition ;

block =
      rule
    | setting
    | lexer rule ;

rule = 
      qualified identifier, '=', expression, ';' ;

setting =
      setting identifier, '=', qualified identifier, ';' ;

lexer rule =   
      qualified identifier, '~', lexer rule expression, ';' ;

expression =   
      term
    | term, '|', expression;

term =   
      factor
    | factor, term;

factor =   
      qualified identifier
    | literal
    | '/', regular expression, '/'
    | repetition
    | optional
    | grouping;

literal =   
      single quote string
    | double quote string;

repetition =   
      '{', expression, '}';

optional =   
      '[', expression, ']';

grouping =   
      '(', expression, ')';

qualified identifier =   
      identifier
    | identifier '.' qualified identifier;

setting identifier  =
      ':', letter, { letter or digit } ;

lexer rule expression =  
      lexer rule term
    | lexer rule term, '|', lexer rule expression ;

lexer rule term       =   
      lexer rule factor
    | lexer rule factor, lexer rule term ;

lexer rule factor     =   
      literal
    | '/' regular expression '/' ;

letter = 
      "A" | "B" | "C" | "D" | "E" | "F" | "G"
    | "H" | "I" | "J" | "K" | "L" | "M" | "N"
    | "O" | "P" | "Q" | "R" | "S" | "T" | "U"
    | "V" | "W" | "X" | "Y" | "Z" | "a" | "b"
    | "c" | "d" | "e" | "f" | "g" | "h" | "i"
    | "j" | "k" | "l" | "m" | "n" | "o" | "p"
    | "q" | "r" | "s" | "t" | "u" | "v" | "w"
    | "x" | "y" | "z" ;
       
digit = 
      "0" | "1" | "2" | "3" | "4" 
    | "5" | "6" | "7" | "8" | "9" ;

letter or digit = 
    letter | digit ;

(* 
    https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form#Extensibility 
    Defining "? /{regular expression}/ ?" where {regular expression} is a POSIX compliant regular expression.
*)

single quote string = 
    ? /['][^']*[']/ ?;

double quote string = 
    ? /["][^"]*["]/ ?;

```

## Meta Lexer Rules

Meta lexer rules ignored throughout the grammar.

Currently the two meta lexer rules are whitespace and comment lexer rules.

```ebnf
whitespace =
      ? /\s+/ ?; 

comment =
      ? /\/\*(\*(?!\/)|[^*])*\*\// ?;
```

## Predefined Settings

Settings are used to add meta information to the parser. The following settings are recognized in pliant.

| Setting | Description | Example Use |
| ------- | ----------- | ----------- |
| start   | denotes the start symbol | :start = S ;|
| ignore  | ignores the lexeme in the gramar | :ignore = whitespace ; |
| trivia  | see [trivia in roslyn](https://github.com/dotnet/roslyn/wiki/Roslyn%20Overview#syntax-trivia) | :trivia = whitespace; |

## Comments

Comments are c style multi line comments. There are no single line c style comments.

```
/* this is a comment */

/* this is 
a multi-line 
comment 
*/
```

## Unicode Support

Pliant supports the full utf-16 character set. When reading the file, specify the encoding as UTF8 or UTF16.

### Loading a grammar  file using UTF8 or UTF16 encoding.

```
string grammarUtf8Source = System.IO.File.ReadAllText("c:\\path\\to\\a\\file.pliant", Encoding.UTF8);

string grammarUtf16Source = System.IO.File.ReadAllText("c:\\path\\to\\a\\file.pliant", Encoding.UTF16);
```

### Planned support for unicode escapes

Unicode escapes of the form \uxxxx where "xxxx" are hex values is a planned support enhancement. Regular expressions and string literals are planned. 