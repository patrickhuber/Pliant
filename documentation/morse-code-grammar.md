# Simple Morse Code Grammar

For definitions of symbols in this grammar see [the grammar](grammars.md) documentation.

```
/* start the grammar with the MorseCode production see grammars.md for details */
:start = MorseCode ;

MorseCode = Character
            | Character MorseCode ;

Character = Letter | Number | Punctuation | Prosign ;

Letter = A | B | C | D | E | F | G | H | I | J | K | L | M 
       | N | O | P | Q | R | S | T | U | V | W | X | Y | Z ;

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

A ~ '.-';
B ~ '-...';
C ~ '-.-.';
D ~ '-..';
E ~ '.';
F ~ '..-.';
H ~ '....';
I ~ '..';
J ~ '.---';
K ~ '-.-';
L ~ '.-..';
M ~ '--';
N ~ '-.';
O ~ '---';
P ~ '.--.';
Q ~ '--.-';
R ~ '.-.';
S ~ '...';
T ~ '-';
U ~ '..-';
V ~ '...-';
W ~ '.--';
X ~ '-..-';
Y ~ '-.--';
Z ~ '--..';

Zero ~ '-----';
One ~ '.----';
Two ~ '..---';
Three ~ '...--';
Four ~ '....-';
Five ~ '.....';
Six ~ '-....';
Seven ~ '--...';
Eight ~ '---..';
Nine ~ '----.';

EndOfWork ~ '...-.-.';
Error ~ '........';
InvitationToTransmit ~ '-.-';
StartingSignal ~ '-.-.-';
NewPageSignal ~ '.-.-.';
Understood ~ '...-.';
Wait ~ '.-...';

Whitespace ~ /\s+/;
:ignore = Whitespace;

```