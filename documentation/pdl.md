# Pliant Definition Language (PDL)

Pliant Definition Language (PDL) is a domain specific language, roughly based off of EBNF, for defining language grammars.

## Core Components

| component   | examples | description |
|-------------|----------|-------------| 
| settings    | :namespace pdl ;        | key value pairs used to set various configuration values for the grammar |
| production  | production = 'a';       | a parsing proudction is a set of parsing rules  |
| rule        | production = 'rule';    | a rule is a set of terminal and non terminals used to define a parse |
| lexer rule  | rule ~ /abc/ ;          | a lexer rule is a deterministic lexical rule used to match characters in a parse |
| comment     | /* this is a comment */ | annotations to the grammar that do not have semantic value and are used as documentation |

## Settings

| Setting   | Usage                         | Description | 
| --------- | ----------------------------- | ----------- |
| ignore    | :ignore <lexer_rule> ;        | Ignore the given tokens               (0..N) |
| namespace | :namespace <namespace_name> ; | Defines the namespace of the grammar  (0..1) |
| start     | :start <production> ;         | Defines the start production          (1)    |
| import    | :import <namespace> ;         | Imports the given namespace           (0..N) |

## Examples

Pliant Definition Language defined in Pliant Definition Language

[embedmd]:# (..\libraries\Pliant\Languages\Pdl\pdl.pdl pdl)

```pdl
:namespace  pdl;
:start      definition;
:ignore     whitespace;
:import     regex;

definition = 
	block
	| block definition ; 

block = 
	rule
	| setting
	| lexer_rule ;
	
rule =
    qualified_identifier '=' expression ';' ;

ruleName = 
    identifier ;

setting =
      setting_identifier '=' qualified_identifier ';' ;

lexer_rule =   
      qualified_identifier '~' lexer_rule_expression ';' ;

expression =   
      term
    | term '|' expression;

term =   
      factor
    | factor term;

factor =   
      qualified_identifier
    | literal
    | regular_expression
    | repetition
    | optional
    | grouping;

literal =   
      single_quote_string
    | double_quote_string;

repetition =   
      '{' expression '}';

optional =   
      '[' expression ']';

grouping =   
      '(' expression ')';

qualified_identifier =   
      identifier
    | identifier '.' qualified_identifier;

setting_identifier  ~
      ':' letter { letter_or_digit } ;

lexer_rule_expression =  
      lexer_rule_term
    | lexer_rule_term '|' lexer_rule_expression ;

lexer_rule_term       =   
      lexer_rule_factor
    | lexer_rule_factor lexer_rule_term ;

lexer_rule_factor     =   
      literal
    | regular_expression ;

regular_expresion ~ '/' regex.regex '/' ;

letter ~ /[a-zA-Z]/ ;
       
digit ~ /[0-9]/ ;

letter_or_digit ~
    letter 
    | digit ;

single_quote_string ~ /['][^']*[']/;

double_quote_string ~ /["][^"]*["]/;

whitespace ~ /[\\s]+/;
```