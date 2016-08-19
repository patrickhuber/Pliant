using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class SetTerminal : BaseTerminal
    {
        private readonly HashSet<char> _characterSet;

        public SetTerminal(params char[] characters)
            : this(new HashSet<char>(characters))
        {
        }

        public SetTerminal(char first)
        {
            _characterSet = new HashSet<char>();
            _characterSet.Add(first);
        }

        public SetTerminal(char first, char second)
            : this(first)
        {
            _characterSet.Add(second);
        }

        public SetTerminal(ISet<char> characterSet)
        {
            _characterSet = new HashSet<char>(characterSet);
        }

        public override bool IsMatch(char character)
        {
            return _characterSet.Contains(character);
        }

        public override string ToString()
        {
            return $"[{string.Join(string.Empty, _characterSet)}]";
        }
    }
}