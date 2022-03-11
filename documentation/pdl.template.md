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