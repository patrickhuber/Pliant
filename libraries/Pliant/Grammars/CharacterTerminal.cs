using System;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class CharacterTerminal : BaseTerminal
    {
        public char Character { get; private set; }

        private readonly Interval[] _intervals;

        public CharacterTerminal(char character)
        {
            Character = character;
            _intervals = new[] { new Interval(character, character) };
        }

        public override int GetHashCode()
        {
            return HashCode.Compute(
                Character.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var terminal = obj as CharacterTerminal;
            if (terminal == null)
                return false;
            return terminal.Character == Character;
        }

        public override bool IsMatch(char character)
        {
            return Character == character;
        }

        public override string ToString()
        {
            return Character.ToString();
        }

        public override Interval[] GetIntervals()
        {
            return _intervals;
        }
    }
}