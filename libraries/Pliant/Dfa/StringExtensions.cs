using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Dfa
{
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a Dfa from the input string.
        /// </summary>
        /// <param name="input">the input string to convert to a dfa</param>
        /// <returns>a IDfaState that represents the start of a DFA for the input string.</returns>
        public static IDfaState ToDfa(this string input)
        {
            var startState = new DfaState();
            IDfaState currentState = startState;

            for (int c = 0; c < input.Length; c++)
            {
                var character = input[c];
                var isFinal = c == input.Length - 1;
                var newState = new DfaState(isFinal);
                var newEdge = new DfaEdge(new Terminal(character), newState);
                currentState.AddEdge(newEdge);
                currentState = newState;
            }

            return startState;
        }
    }
}
