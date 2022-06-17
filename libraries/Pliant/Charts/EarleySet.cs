﻿using Pliant.Collections;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public class EarleySet : IEarleySet
    {
        private static readonly INormalState[] EmptyNormalStates = { };
        private static readonly ITransitionState[] EmptyTransitionStates = { };
        private UniqueList<INormalState> _predictions;
        private UniqueList<INormalState> _scans;
        private UniqueList<INormalState> _completions;
        private UniqueList<ITransitionState> _transitions;

        public IReadOnlyList<INormalState> Predictions
        {
            get
            {
                if (_predictions is null)
                    return EmptyNormalStates;
                return _predictions;
            } 
        }

        public IReadOnlyList<INormalState> Scans
        {
            get
            {
                if (_scans is null)
                    return EmptyNormalStates;
                return _scans;
            }
        }

        public IReadOnlyList<INormalState> Completions
        {
            get
            {
                if (_completions is null)
                    return EmptyNormalStates;
                return _completions;
            }
        }

        public IReadOnlyList<ITransitionState> Transitions
        {
            get
            {
                if (_transitions is null)
                    return EmptyTransitionStates;
                return _transitions;
            }
        }

        public int Location { get; private set; }

        public EarleySet(int location)
        {
            Location = location;
        }

        public bool Contains(StateType stateType, IDottedRule dottedRule, int origin)
        {
            if (stateType != StateType.Normal)
                return false;

            var hashCode = NormalStateHashCodeAlgorithm.Compute(dottedRule, origin);
            if (dottedRule.IsComplete)
                return CompletionsContainsHash(hashCode);

            var currentSymbol = dottedRule.PostDotSymbol;
            if (currentSymbol.SymbolType == SymbolType.NonTerminal)
                return PredictionsContainsHash(hashCode);

            return ScansContainsHash(hashCode);
        }

        private bool CompletionsContainsHash(int hashCode)
        {
            if (_completions is null)
                return false;
            return _completions.ContainsHash(hashCode);
        }

        private bool PredictionsContainsHash(int hashCode)
        {
            if (_predictions is null)
                return false;
            return _predictions.ContainsHash(hashCode);
        }

        private bool ScansContainsHash(int hashCode)
        {
            if (_scans is null)
                return false;
            return _scans.ContainsHash(hashCode);
        }

        public bool Enqueue(IState state)
        {
            if (state.StateType == StateType.Transitive)
                return EnqueueTransition(state as ITransitionState);
            
            return EnqueueNormal(state, state as INormalState);
        }

        private bool EnqueueNormal(IState state, INormalState normalState)
        {
            var dottedRule = state.DottedRule;
            if (dottedRule.IsComplete)
                return AddUniqueCompletion(normalState);
            
            var currentSymbol = dottedRule.PostDotSymbol;
            if (currentSymbol.SymbolType == SymbolType.NonTerminal)
                return AddUniquePrediction(normalState);

            return AddUniqueScan(normalState);
        }

        private bool AddUniqueCompletion(INormalState normalState)
        {
            if (_completions is null)
                _completions = new UniqueList<INormalState>();
            return _completions.AddUnique(normalState);
        }

        private bool AddUniqueScan(INormalState normalState)
        {
            if (_scans is null)
                _scans = new UniqueList<INormalState>();
            return _scans.AddUnique(normalState);
        }

        private bool AddUniquePrediction(INormalState normalState)
        {
            if (_predictions is null)
                _predictions = new UniqueList<INormalState>();
            return _predictions.AddUnique(normalState);
        }

        private bool EnqueueTransition(ITransitionState transitionState)
        {
            if (_transitions is null)
                _transitions = new UniqueList<ITransitionState>();
            return _transitions.AddUnique(transitionState);
        }

        public ITransitionState FindTransitionState(ISymbol searchSymbol)
        {
            for (int t = 0; t < Transitions.Count; t++)
            {
                var transitionState = Transitions[t] as TransitionState;
                if (transitionState.Recognized.Equals(searchSymbol))
                    return transitionState;
            }
            return null;
        }

        /// <summary>
        /// Finds a prediction where the post dot symbol is the search symbol
        /// </summary>
        /// <param name="searchSymbol"></param>
        /// <returns></returns>
        public IState FindSourceState(ISymbol searchSymbol)
        {
            var sourceItemCount = 0;
            IState sourceItem = null;

            for (int s = 0; s < Predictions.Count; s++)
            {
                var state = Predictions[s];
                if (state.IsSource(searchSymbol))
                {
                    var moreThanOneSourceItemExists = sourceItemCount > 0;
                    if (moreThanOneSourceItemExists)
                        return null;
                    sourceItemCount++;
                    sourceItem = state;
                }
            }
            return sourceItem;
        }        
    }
}