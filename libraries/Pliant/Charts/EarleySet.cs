using Pliant.Collections;
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
        private Dictionary<ISymbol, List<INormalState>> _sources;
        private Dictionary<ISymbol, ITransitionState> _cached;
        private Dictionary<ISymbol, List<INormalState>> _reductions;

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
            _completions ??= new UniqueList<INormalState>();
            if (!_completions.AddUnique(normalState))
                return false;

            var symbol = normalState.DottedRule.Production.LeftHandSide;

            _reductions ??= new Dictionary<ISymbol, List<INormalState>>();
            if (!_reductions.TryGetValue(symbol, out var list))
                list = _reductions[symbol] = new List<INormalState>();

            list.Add(normalState);

            return true;
        }

        private bool AddUniqueScan(INormalState normalState)
        {
            if (_scans is null)
                _scans = new UniqueList<INormalState>();
            return _scans.AddUnique(normalState);
        }

        private bool AddUniquePrediction(INormalState normalState)
        {
            _predictions ??= new UniqueList<INormalState>();
            if (!_predictions.AddUnique(normalState))
                return false;

            // skip any null states
            if (normalState.DottedRule.Production.RightHandSide.Count == 0)
                return true;

            // cache the prediction
            _sources ??= new Dictionary<ISymbol, List<INormalState>>();
            if (!_sources.TryGetValue(normalState.DottedRule.PostDotSymbol, out List<INormalState> predictions))
                predictions = _sources[normalState.DottedRule.PostDotSymbol] = new List<INormalState>();

            predictions.Add(normalState);
            return true;
        }

        private bool EnqueueTransition(ITransitionState transitionState)
        {
            if (_transitions is null)
                _transitions = new UniqueList<ITransitionState>();

            if (!_transitions.AddUnique(transitionState))
                return false;

            _cached ??= new Dictionary<ISymbol, ITransitionState>();
            _cached[transitionState.Recognized] = transitionState;
            return true;
        }

        public ITransitionState FindTransitionState(ISymbol searchSymbol)
        {
            if (_cached is null)
                return null;
            if (_cached.TryGetValue(searchSymbol, out var transitionState))
                return transitionState;
            return null;
        }

        /// <summary>
        /// Finds a prediction where the post dot symbol is the search symbol
        /// </summary>
        /// <param name="searchSymbol"></param>
        /// <returns></returns>
        public IReadOnlyList<INormalState> FindSourceStates(ISymbol searchSymbol)
        {
            if (searchSymbol is null)
                return EmptyNormalStates;

            if (_sources is null)
                return EmptyNormalStates;

            if (!_sources.TryGetValue(searchSymbol, out var list))
                return EmptyNormalStates;

            return list;
        }

        public IReadOnlyList<INormalState> FindReductions(ISymbol symbol)
        {
            if (symbol is null)
                return EmptyNormalStates;

            if (_reductions is null)
                return EmptyNormalStates;

            if (!_reductions.TryGetValue(symbol, out var list))
                return EmptyNormalStates;
            return list;
        }
    }
}