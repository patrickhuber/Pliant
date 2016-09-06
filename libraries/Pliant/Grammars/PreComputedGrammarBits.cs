using System;
using Pliant.Grammars;
using Pliant.Collections;
using System.Collections.Generic;
using System.Collections;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class PreComputedGrammarBits
    {
        private IGrammar _grammar;

        private List<PreComputedState> _states;
        private UniqueList<ISymbol> _symbols;

        private BitArray _nullableStates;
        private BitMatrix _predictionTransitions;
        private Dictionary<ISymbol, BitMatrix> _symbolTransitions;

        public PreComputedGrammarBits(IGrammar grammar)
        {
            _grammar = grammar;            
            PreComputeAllStates(_grammar);
            _nullableStates = new BitArray(_states.Count);
            _predictionTransitions = new BitMatrix(_states.Count);
            _symbolTransitions = new Dictionary<ISymbol, BitMatrix>();
            CreateTransitionsMatricies(_states, _nullableStates, _predictionTransitions, _symbolTransitions);
        }

        private void PreComputeAllStates(IGrammar grammar)
        {
            _states = new List<PreComputedState>();
            _symbols = new UniqueList<ISymbol>();
            for (int p = 0; p <grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                _symbols.AddUnique(production.LeftHandSide);
                for (int s = 0; s <= production.RightHandSide.Count; s++)
                {
                    var preComputedState = new PreComputedState(production, s);
                    _states.Add(preComputedState);
                    if (s == production.RightHandSide.Count)
                        break;
                    _symbols.AddUnique(production.RightHandSide[s]);
                }
            }
        }

        private static void CreateTransitionsMatricies(
            List<PreComputedState> states,
            BitArray nullableStates,
            BitMatrix predictionTransitions,
            Dictionary<ISymbol, BitMatrix> symbolTransitions)
        {
            for (int i = 0; i < states.Count; i++)
            {
                var source = states[i];

                // is the rule nullable
                if (source.Production.RightHandSide.Count == 0 && !nullableStates[i])
                    nullableStates[i] = true;

                // skip completed states
                var isComplete = source.Production.RightHandSide.Count == source.Position;
                if (isComplete)
                    continue;

                var postDotSymbol = source.Production.RightHandSide[source.Position];

                for (int j = 0; j < states.Count; j++)
                {
                    var target = states[j];
                    
                    // prediction
                    if (target.Position == 0
                        && postDotSymbol.SymbolType == SymbolType.NonTerminal
                        && target.Production.LeftHandSide.Equals(postDotSymbol))
                    {
                        predictionTransitions[i][j] = true;
                    }

                    // is a scan or completion
                    // PERF: do this after prediction fails because production comparison 
                    //     needs to compare the symbols of the production as well
                    else if (source.Position + 1 == target.Position 
                        && source.Production.Equals(target.Production))
                    {
                        BitMatrix symbolTransition = null;
                        if (!symbolTransitions.TryGetValue(postDotSymbol, out symbolTransition))
                        {
                            symbolTransition = new BitMatrix(states.Count);
                            symbolTransitions[postDotSymbol] = symbolTransition;
                        }
                        symbolTransition[i][j] = true;
                    }
                }                 
            }
        }
    }
}
