# Simple Morse Code Grammar

```
MorseCode = Character
            | Character MorseCode ;

Character = Letter | Number | Punctuation | Prosign ;

Letter = A | B | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q | R | S | T | U | V | W | X | Y | Z ;

Number = Zero 
        | One
        | Two
        | Three
        | Four
        | Five
        | Six
        | Seven
        | Eight
        | Nine ;

Punctuation =   Period 
                | Comma 
                | QuestionMark 
                | Apostrophe 
                | ExclamationPoint 
                | SlashOrFractionBar 
                | OpenParenthesis 
                | CloseParenthesis 
                | Ampersand 
                | Colon
                | Semicolon
                | DoubleDash 
                | PlusSign
                | HyphenOrMinusSign 
                | Underscore 
                | QuotationMark 
                | DollarSign 
                | AtSign ;

Prosign =   EndOfWork 
            | Error
            | InvitationToTransmit 
            | StartingSignal 
            | NewPageSignal 
            | Understood 
            | Wait ;

A ~ Dot Dash;
B ~ Dash Dot Dot Dot;
C ~ Dash Dot Dash Dot;
D ~ Dash Dot Dot;
E ~ Dot;
F ~ Dot Dot Dash Dot;
H ~ Dot Dot Dot Dot;
I ~ Dot Dot;
J ~ Dot Dash Dash Dash ;
K ~ Dash Dot Dash;
L ~ Dot Dash Dot Dot;
M ~ Dash Dash;
N ~ Dash Dot;
O ~ Dash Dash Dash;
P ~ Dot Dash Dash Dot;
Q ~ Dash Dash Dot Dash;
R ~ Dot Dash Dot;
S ~ Dot Dot Dot;
T ~ Dash;
U ~ Dot Dot Dash;
V ~ Dot Dot Dot Dash;
W ~ Dot Dash Dash;
X ~ Dash Dot Dot Dash;
Y ~ Dash Dot Dash Dash;
Z ~ Dash Dash Dot Dot;

Zero ~ Dash Dash Dash Dash Dash ;
One ~ Dot Dash Dash Dash Dash ;
Two ~ Dot Dot Dash Dash Dash ;
Three ~ Dot Dot Dot Dash Dash ;
Four ~ Dot Dot Dot Dot Dash;
Five ~ Dot Dot Dot Dot Dot;
Six ~ Dash Dot Dot Dot Dot;
Seven ~ Dash Dash Dot Dot Dot;
Eight ~ Dash Dash Dash Dot Dot;
Nine ~ Dash Dash Dash Dash Dot;

EndOfWork = Dot Dot Dot Dash Dot Dash Dot;
Error = Dot Dot Dot Dot Dot Dot Dot Dot;
InvitationToTransmit = Dash Dot Dash ;
StartingSignal = Dash Dot Dash Dot Dash ;
NewPageSignal = Dot Dash Dot Dash Dot;
Understood = Dot Dot Dot Dash Dot;
Wait = Dot Dash Dot Dot Dot;


Dot ~ '.' ;
Dash ~ '-' ;
Whitespace ~ /\w+/;
:ignore Whitespace;

```