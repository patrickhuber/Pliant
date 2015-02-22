using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class CharacterClassBuilder: ICharacterClassBuilder
    {
        private IList<ICharacterClass> _characterClasses;

        public CharacterClassBuilder()
        {
            _characterClasses = new List<ICharacterClass>();
        }

        public ICharacterClassBuilder Range(char start, char end)
        {
            _characterClasses.Add(new DelegateCharacterClass(c => start <= c && c <= end));
            return this;
        }

        public ICharacterClassBuilder Whitespace()
        {
            _characterClasses.Add(new DelegateCharacterClass(c=>char.IsWhiteSpace(c)));
            return this;
        }

        public ICharacterClassBuilder Word()
        {            
            throw new NotImplementedException();
        }

        public ICharacterClassBuilder Digit()
        {
            _characterClasses.Add(new DelegateCharacterClass(c => char.IsDigit(c)));
            return this;        
        }

        public void Any()
        {
            throw new NotImplementedException();
        }

        public IList<ICharacterClass> GetCharacterClasses()
        {
            return _characterClasses;
        }
    }
}
