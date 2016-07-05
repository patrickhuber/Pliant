﻿using Pliant.Grammars;

namespace Pliant.Automata
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

            for (var c = 0; c < input.Length; c++)
            {
                var character = input[c];
                var isFinal = c == input.Length - 1;
                var newState = new DfaState(isFinal);
                var newEdge = new DfaTransition(new CharacterTerminal(character), newState);
                currentState.AddTransition(newEdge);
                currentState = newState;
            }

            return startState;
        }
    }
}