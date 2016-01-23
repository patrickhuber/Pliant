using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class SetTerminal : BaseTerminal, ITerminal
    {
        private ISet<char> _characterSet;

        public SetTerminal(params char[] characters)
            : this(new HashSet<char>(characters))
        {
        }

        public SetTerminal(ISet<char> characterSet)
        {
            _characterSet = characterSet;
        }

        public override bool IsMatch(char character)
        {
            return _characterSet.Contains(character);
        }

        public override string ToString()
        {
            return string.Join(", ", string.Format("'{0}'", _characterSet));
        }
    }
}