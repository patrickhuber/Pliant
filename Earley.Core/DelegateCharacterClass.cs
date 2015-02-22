using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class DelegateCharacterClass : ICharacterClass
    {
        private Func<char, bool> _isMatch;

        public DelegateCharacterClass(Func<char, bool> isMatch)
        { 
        }

        public bool IsMatch(char character)
        {
            return _isMatch(character);
        }
    }
}
