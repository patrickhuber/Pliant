# Regular Expression Syntax Documentation 

You can use regular expressions in lexer rules to form lexemes for the ParseEngine. 

Regular expressions follow a simplified subset of the regular expressions provided by the .NET framework. For example, Backreference, Anchors and Substitions are not supported. 

## Grammar

The supported language for regular expressions in Pliant is defined by the following grammar. 

```ebnf
regex =   
        expression 
    |   "^", expression 
    |   expression, "$"
    |   "^", expression, "$";

(* an empty rule, or lambda rule is denoted by alteration with no body *)
expression =  
        term 
    |   term, '|', expression
    | ;

term = 
        factor
    |   factor, term ;

factor = 
        atom 
    |   atom, iterator;

iterator = 
    '*' | '+' | '?';

atom =
        . 
    |   character
    |   "(", expression, ")"
    |   set ;

set =
        positive set
    |   negative set ;

positive set =
        "[", character class, "]";

negative set = 
        "[^", character class, "]";

character class = 
        character range 
    |   character range, character class ;

character range =
        character class character 
    |   character class character, "-", character class character ;

character =
        not meta character 
    |   escape sequence ;

character class character =
        not close bracket character
    |   escape sequence ;

(* 
    https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form#Extensibility 
    Defining "? /{regular expression}/ ?" where {regular expression} is a POSIX compliant regular expression.
*)
not meta character =
    ? /[^.^$()[\]+*?\\\/]/ ?;

not close bracket character = 
    ? /[^\]]/ ?;

escape sequence = 
    ? /[\\]./ ?;
```

## Character Classes

The following character escapes are translated into Terminal objects upon recognition.

| Character Class  | Description | Pattern | Matches | Terminal | 
| ------------- | ------------- | ------------- | ------------- | ------------- |
| ```[``` character_group ```]``` | Matches the set of characters | ```[ae]``` | ```"a" in "gray"``` | new SetTerminal('a', 'e') |
| ```[^``` character_group ```]``` | Matches the set of characters not in the character_grup | ```[^gra]``` | ```"y" in "gray"``` | new NegationTerminal(new SetTerminal('g', 'r', 'a')) |
| ```[``` first ```-``` last ```]```| Character Range: matches any single character in the range from *first* to *last* | ```[A-Z]``` | ```"A" in "12A34"```  | |
| ```\s```  | Matches any white-space character.  | ```\s```  | ```" " in "ID A1.3"```  | new WhitespaceTerminal() |
| ```\d```  | Matches any decimal digit.  | ```\d```  | ```"4" in "4 = IV"```  | new DigitTerminal() |
| ```\w```  | Matches any word character. | ```\w```|  ```"I" in "0I123"```| new WordTerminal() | 
| ```\D```  | Matches any character other than a decimal digit. | ```\D``` | ```"D" in "4D"```| new NegationTerminal(new DigitTerminal))
| ```\S```  | Matches any non-white-space character. | ```\S``` | ```"D" in " D "``` | new NegationTerminal(new WhitespaceTerminal())|
| ```\W```  | Matches any non-word character. | ```\W``` | ```"." in "1ab." ``` | new NegationTerminal(new WordTerminal())|

## Anchors

| Assertion | Description | Pattern | Matches |
| --------- | ----------- | ------- | ------- |
| ```^``` | (parses but not implemented) The match must start at the beginning of a string. | | |
| ```$``` | (parses but not implemented) The match must occur at the end of the string or before ```\n``` at the end of the line or string. | | |

## Grouping Constructs

| Grouping Construct | Description |
| ------------------ | ----------- | 
| ```(``` subexpression ```)``` | Captures the matched subexpression |

## Quantifiers 

| Quantifier | Description | Pattern | Matches | 
| ---------- | ----------- | ------- | ------- | 
| ```*```    | Matches the previous element zero or more times. | ```\d*\.\d``` | ".0", "19.9", "219.9" |
| ```+```    | Matches the previous element one or more times. | ```"be+"``` | "bee" in "been", "be" in "bent" |
| ```?```    | Matches the previous element zero or one time. | ```"rai?n"```| "ran", "rain" |