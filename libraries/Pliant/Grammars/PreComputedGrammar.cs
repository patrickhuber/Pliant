using Pliant.Collections;
using System.Collections.Generic;
using Pliant.Utilities;
using System;

namespace Pliant.Grammars
{
    public class PreComputedGrammar
    {
        public IGrammar Grammar { get; private set; }

        public DottedRuleSet Start { get; private set; }

        private readonly ProcessOnceQueue<DottedRuleSet> _dottedRuleSetQueue;

        private readonly Dictionary<DottedRuleSet, DottedRuleSet> _dottedRuleSets;
        
        public PreComputedGrammar(IGrammar grammar)
        {
            _dottedRuleSetQueue = new ProcessOnceQueue<DottedRuleSet>();
            _dottedRuleSets = new Dictionary<DottedRuleSet, DottedRuleSet>();

            Grammar = grammar;
            
            var startStates = Initialize(Grammar);
            Start = AddNewOrGetExistingDottedRuleSet(startStates);
            ProcessDottedRuleSetQueue();
        }
        
        private void ProcessDottedRuleSetQueue()
        {
            while (_dottedRuleSetQueue.Count > 0)
            {
                // assume the closure has already been captured
                var frame = _dottedRuleSetQueue.Dequeue();
                ProcessSymbolTransitions(frame);

                // capture the predictions for the frame
                var predictedStates = GetPredictedStates(frame);

                // if no predictions, continue
                if (predictedStates.Count == 0)
                    continue;

                // assign the null transition
                // only process symbols on the null frame if it is new
                DottedRuleSet nullDottedRuleSet;
                if (!TryGetOrCreateDottedRuleSet(predictedStates, out nullDottedRuleSet))
                    ProcessSymbolTransitions(nullDottedRuleSet);

                frame.NullTransition = nullDottedRuleSet;
            }
        }

        private SortedSet<IDottedRule> Initialize(IGrammar grammar)
        {
            var pool = SharedPools.Default<SortedSet<IDottedRule>>();
            
            var startStates = pool.AllocateAndClear();
            var startProductions = grammar.StartProductions();

            for (var p = 0; p < startProductions.Count; p++)
            {
                var production = startProductions[p];
                var state = GetPreComputedState(production, 0);
                startStates.Add(state);
            }

            var confirmedStates = GetConfirmedStates(startStates);

            pool.ClearAndFree(startStates);
            return confirmedStates;
        }

        private IDottedRule GetPreComputedState(IProduction production, int position)
        {
            return Grammar.DottedRules.Get(production, position);
        }

        private SortedSet<IDottedRule> GetConfirmedStates(SortedSet<IDottedRule> states)
        {
            var pool = SharedPools.Default<Queue<IDottedRule>>();

            var queue = pool.AllocateAndClear();
            var closure = new SortedSet<IDottedRule>();

            foreach (var state in states)
                if (closure.Add(state))
                    queue.Enqueue(state);

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (IsComplete(state))
                    continue;

                var production = state.Production;
                for (var s = state.Position; s < state.Production.RightHandSide.Count; s++)
                {
                    var postDotSymbol = production.RightHandSide[s];
                    if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                        break;

                    var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;
                    if (!Grammar.IsTransativeNullable(nonTerminalPostDotSymbol))
                        break;

                    var preComputedState = GetPreComputedState(production, s + 1);
                    if (closure.Add(preComputedState))
                        queue.Enqueue(preComputedState);
                }
            }
            pool.ClearAndFree(queue);
            return closure;
        }

        private SortedSet<IDottedRule> GetPredictedStates(DottedRuleSet frame)
        {
            var pool = SharedPools.Default<Queue<IDottedRule>>();

            var queue = pool.AllocateAndClear();
            var closure = new SortedSet<IDottedRule>();

            for (int i = 0; i < frame.Data.Count; i++)
            {
                var state = frame.Data[i];
                if (!IsComplete(state))
                    queue.Enqueue(state);
            }

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (IsComplete(state))
                    continue;
                
                var postDotSymbol = GetPostDotSymbol(state);
                if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                    continue;

                var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;
                if (Grammar.IsTransativeNullable(nonTerminalPostDotSymbol))
                {
                    var preComputedState = GetPreComputedState(state.Production, state.Position + 1);
                    if (!frame.Contains(preComputedState))
                        if (closure.Add(preComputedState))
                            if (!IsComplete(preComputedState))
                                queue.Enqueue(preComputedState);
                }

                var predictions = Grammar.RulesFor(nonTerminalPostDotSymbol);
                for (var p = 0; p < predictions.Count; p++)
                {
                    var prediction = predictions[p];
                    var preComputedState = GetPreComputedState(prediction, 0);
                    if (frame.Contains(preComputedState))
                        continue;
                    if (!closure.Add(preComputedState))
                        continue;
                    if (!IsComplete(preComputedState))
                        queue.Enqueue(preComputedState);
                }
            }

            pool.ClearAndFree(queue);
            return closure;
        }

        private DottedRuleSet AddNewOrGetExistingDottedRuleSet(SortedSet<IDottedRule> states)
        {
            var dottedRuleSet = new DottedRuleSet(states);
            DottedRuleSet outDottedRuleSet;
            if (_dottedRuleSets.TryGetValue(dottedRuleSet, out outDottedRuleSet))
                return outDottedRuleSet;
            outDottedRuleSet = dottedRuleSet;
            _dottedRuleSets[dottedRuleSet] = outDottedRuleSet;
            _dottedRuleSetQueue.Enqueue(outDottedRuleSet);
            return outDottedRuleSet;
        }

        private bool TryGetOrCreateDottedRuleSet(SortedSet<IDottedRule> states, out DottedRuleSet outDottedRuleSet)
        {
            var dottedRuleSet = new DottedRuleSet(states);
            if (_dottedRuleSets.TryGetValue(dottedRuleSet, out outDottedRuleSet))
                return true;
            outDottedRuleSet = dottedRuleSet;
            _dottedRuleSets[dottedRuleSet] = outDottedRuleSet;
            _dottedRuleSetQueue.Enqueue(outDottedRuleSet);
            return false;
        }

        private void ProcessSymbolTransitions(DottedRuleSet frame)
        {
            var pool = SharedPools.Default<Dictionary<ISymbol, SortedSet<IDottedRule>>>();
            var transitions = pool.AllocateAndClear();

            for (int i = 0; i < frame.Data.Count; i++)
            {
                var nfaState = frame.Data[i];                
                if (IsComplete(nfaState))
                    continue;
                
                var postDotSymbol = GetPostDotSymbol(nfaState);
                var targetStates = transitions.AddOrGetExisting(postDotSymbol);
                var nextRule = GetPreComputedState(nfaState.Production, nfaState.Position + 1);

                targetStates.Add(nextRule);
            }

            foreach (var symbol in transitions.Keys)
            {
                var confirmedStates = GetConfirmedStates(transitions[symbol]);
                var valueDottedRuleSet = AddNewOrGetExistingDottedRuleSet(confirmedStates);
                frame.AddTransistion(symbol, valueDottedRuleSet);
            }

            pool.ClearAndFree(transitions);
        }

        private static ISymbol GetPostDotSymbol(IDottedRule state)
        {
            return state.Production.RightHandSide[state.Position];
        }

        private static bool IsComplete(IDottedRule state)
        {
            return state.Position == state.Production.RightHandSide.Count;
        }
    }
}
