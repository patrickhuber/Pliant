using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class TableDfa
    {
        private Dictionary<int, Dictionary<char, int>> _table;
        private HashSet<int> _finalStates;

        public TableDfa(int start)
        {
            Start = start;
            _table = new Dictionary<int, Dictionary<char, int>>();
            _finalStates = new HashSet<int>();
        }

        public void AddTransition(int source, char character, int target)
        {
            var sourceTransitions = _table.AddOrGetExisting(source);
            sourceTransitions[character] = target;
        }

        public void SetFinal(int state, bool isFinal)
        {
            if (isFinal)            
                _finalStates.Add(state);            
            else
                _finalStates.Remove(state);
        }

        public bool IsFinal(int state)
        {
            return _finalStates.Contains(state);
        }

        public int Start { get; private set; }

        public int? Transition(int source, char character)
        {
            Dictionary<char, int> sourceTransitions = null;
            if (!_table.TryGetValue(source, out sourceTransitions))
                return null;
            var target = default(int);
            if (sourceTransitions.TryGetValue(character, out target))
                return target;
            return null;
        }
    }
}
