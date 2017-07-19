# Grammar Syntax Documentation

Grammars in Pliant are defined by an bnf dialect similar to abnf and ebnf. Some additional control statements have been added for ignoring whitespace and specifying start productions.

# Grammar of Grammars

The supported language for grammars in Pliant is defined by the following BNF grammar. 

```
Definition          =   Block
                        | Block Definition

Block               =   Rule
                        | Setting
                        | LexerRule

Rule                =   QualifiedIdentifier '=' Expression ';'

Setting             =   SettingIdentifier '=' QualifiedIdentifier ';'

LexerRule           =   QualifiedIdentifier '~' LexerRuleExpression ';'

Expression          =   Term
                        | Term '|' Expression

Term                =   Factor
                        | Factor Term

Factor              =   QualifiedIdentifier
                        | Literal
                        | '/' RegularExpression '/'
                        | Repetition
                        | Optional
                        | Grouping

Literal             =   SingleQuoteStringLexerRule
                        | DoubleQuoteStringLexerRule

Repetition          =   '{' Expression '}'

Optional            =   '[' Expression ']'

Grouping            =   '(' Expression ')'

QualifiedIdentifier =   Identifier
                        | Identifier '.' QualifiedIdentifier

LexerRuleExpression =   LexerRuleTerm
                        | LexerRuleTerm '|' LexerRuleExpression

LexerRuleTerm       =   LexerRuleFactor
                        | LexerRuleFactor LexerRuleTerm

LexerRuleFactor     =   Literal
                        | '/' RegularExpression '/'

SingleQuoteStringLexerRule ~ /['][^']*[']/

DoubleQuoteStringLexerRule ~ /["][^"]*["]/

```

