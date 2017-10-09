# Regular Expression Syntax Documentation 

You can use regular expressions in lexer rules to form lexemes for the ParseEngine. 

Regular expressions follow a simplified subset of the regular expressions provided by the .NET framework. For example, Backreference, Anchors and Substitions are not supported. 

## Grammar

The supported language for regular expressions in Pliant is defined by the following grammar. 

```
Regex                      ->   Expression |
                                '^' Expression |
                                Expression '$' |
                                '^' Expression '$'
 
Expresion                  ->   Term |
                                Term '|' Expression
                                λ

Term                       ->   Factor |
                                Factor Term

Factor                     ->   Atom |
                                Atom Iterator

Iterator                   ->   '*' | '+' | '?'

Atom                       ->   . |
                                Character |
                                '(' Expression ')' |
                                Set

Set                        ->   PositiveSet |
                                NegativeSet

PositiveSet                ->   '[' CharacterClass ']'

NegativeSet                ->   "[^" CharacterClass ']'

CharacterClass             ->   CharacterRange |
                                CharacterRange CharacterClass

CharacterRange             ->   CharacterClassCharacter |
                                CharacterClassCharacter '-'
                                CharacterClassCharacter

Character                  ->   NotMetaCharacter |
                                EscapeSequence

CharacterClassCharacter    ->   NotCloseBracketCharacter |
                                EscapeSequence
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
| ```$``` | (parses but not implemented) The match must occur at the end of the string or before ```\n``` at teh eend of the line or string. | | |

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