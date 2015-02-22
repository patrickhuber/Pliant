using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public interface ICharacterClassBuilder
    {
        //void CharacterClass(string @class);

        ICharacterClassBuilder Range(char start, char end);

        ICharacterClassBuilder Whitespace();

        ICharacterClassBuilder Word();

        ICharacterClassBuilder Digit();

        void Any();
    }
}
