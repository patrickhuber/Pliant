using Pliant.Collections;
using System.Collections.Generic;
using Pliant.Utilities;
using System;

namespace Pliant.Grammars
{
    public class PreComputedGrammar
    {
        public IGrammar Grammar { get; private set; }

        public Frame Start { get; private set; }

        private readonly ProcessOnceQueue<Frame> _frameQueue;

        private readonly Dictionary<Frame, Frame> _frames;

        private readonly HashSet<ISymbol> _rightRecursiveSymbols;

        private readonly Dictionary<IProduction, Dictionary<int, PreComputedState>> _states;

        public PreComputedGrammar(IGrammar grammar)
        {
            _frameQueue = new ProcessOnceQueue<Frame>();
            _frames = new Dictionary<Frame, Frame>();

            Grammar = grammar;
            Dictionary<ISymbol, UniqueList<ISymbol>> symbolPaths = null;

            CreateStatesSymbolsAndSymbolPaths(grammar, out _states, out symbolPaths);

            var startStates = Initialize(Grammar);
            Start = AddNewFrameOrGetExistingFrame(startStates);
            ProcessFrameQueue();

            _rightRecursiveSymbols = CreateRightRecursiveLookup(Grammar, _states, symbolPaths);
        }

        private void ProcessFrameQueue()
        {
            while (_frameQueue.Count > 0)
            {
                // assume the closure has already been captured
                var frame = _frameQueue.Dequeue();
                ProcessSymbolTransitions(frame);

                // capture the predictions for the frame
                var predictedStates = GetPredictedStates(frame);

                // if no predictions, continue
                if (predictedStates.Count == 0)
                    continue;

                // assign the null transition
                // only process symbols on the null frame if it is new
                Frame nullFrame;
                if (!TryGetExistingFrameOrCreateNew(predictedStates, out nullFrame))
                    ProcessSymbolTransitions(nullFrame);

                frame.NullTransition = nullFrame;
            }
        }

        private static void CreateStatesSymbolsAndSymbolPaths(
            IGrammar grammar,
            out Dictionary<IProduction, Dictionary<int, PreComputedState>> states,
            out Dictionary<ISymbol, UniqueList<ISymbol>> symbolPaths)
        {
            states = new Dictionary<IProduction, Dictionary<int, PreComputedState>>();
            symbolPaths = new Dictionary<ISymbol, UniqueList<ISymbol>>();

            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                var stateIndex = states.AddOrGetExisting(production);
                var leftHandSide = production.LeftHandSide;
                var symbolPath = symbolPaths.AddOrGetExisting(leftHandSide);

                for (var s = 0; s <= production.RightHandSide.Count; s++)
                {
                    var preComputedState = new PreComputedState(production, s);
                    stateIndex.Add(s, preComputedState);

                    if (s < production.RightHandSide.Count)
                    {
                        var postDotSymbol = production.RightHandSide[s];
                        symbolPath.AddUnique(postDotSymbol);
                    }
                }
            }
        }
        
        private static HashSet<ISymbol> CreateRightRecursiveLookup(
            IGrammar grammar, 
            Dictionary<IProduction, Dictionary<int, PreComputedState>> states,
            Dictionary<ISymbol, UniqueList<ISymbol>> symbolPaths)
        {
            var hashSet = new HashSet<ISymbol>();
            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                var stateIndex = states[production];
                var position = production.RightHandSide.Count;
                var completed = stateIndex[position];
                var symbolPath = symbolPaths[production.LeftHandSide];

                for (var s = position; s > 0; s--)
                {
                    var preDotSymbol = production.RightHandSide[s - 1];
                    if (preDotSymbol.SymbolType != SymbolType.NonTerminal)
                        break;

                    var preDotNonTerminal = preDotSymbol as INonTerminal;
                    if (symbolPath.Contains(preDotNonTerminal))
                    {
                        hashSet.Add(production.LeftHandSide);
                        break;
                    }
                    if (!grammar.IsNullable(preDotNonTerminal))
                        break;
                }
            }
            return hashSet;
        }

        public bool IsRightRecursive(ISymbol symbol)
        {
            return _rightRecursiveSymbols.Contains(symbol);
        }

        private SortedSet<PreComputedState> Initialize(IGrammar grammar)
        {
            var pool = SharedPools.Default<SortedSet<PreComputedState>>();
            
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

        private  PreComputedState GetPreComputedState(IProduction production, int position)
        {
            return _states[production][position];
        }

        private SortedSet<PreComputedState> GetConfirmedStates(SortedSet<PreComputedState> states)
        {
            var pool = SharedPools.Default<Queue<PreComputedState>>();

            var queue = pool.AllocateAndClear();
            var closure = new SortedSet<PreComputedState>();

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
                    if (!Grammar.IsNullable(nonTerminalPostDotSymbol))
                        break;

                    var preComputedState = GetPreComputedState(production, s + 1);
                    if (closure.Add(preComputedState))
                        queue.Enqueue(preComputedState);
                }
            }
            pool.ClearAndFree(queue);
            return closure;
        }

        private SortedSet<PreComputedState> GetPredictedStates(Frame frame)
        {
            var pool = SharedPools.Default<Queue<PreComputedState>>();

            var queue = pool.AllocateAndClear();
            var closure = new SortedSet<PreComputedState>();

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
                if (Grammar.IsNullable(nonTerminalPostDotSymbol))
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

        private Frame AddNewFrameOrGetExistingFrame(SortedSet<PreComputedState> states)
        {
            var frame = new Frame(states);
            Frame outFrame;
            if (_frames.TryGetValue(frame, out outFrame))
                return outFrame;
            outFrame = frame;
            _frames[frame] = outFrame;
            _frameQueue.Enqueue(outFrame);
            return outFrame;
        }

        private bool TryGetExistingFrameOrCreateNew(SortedSet<PreComputedState> states, out Frame outFrame)
        {
            var newFrame = new Frame(states);
            if (_frames.TryGetValue(newFrame, out outFrame))
                return true;
            outFrame = newFrame;
            _frames[newFrame] = outFrame;
            _frameQueue.Enqueue(outFrame);
            return false;
        }

        private void ProcessSymbolTransitions(Frame frame)
        {
            var pool = SharedPools.Default<Dictionary<ISymbol, SortedSet<PreComputedState>>>();
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
                var valueFrame = AddNewFrameOrGetExistingFrame(confirmedStates);
                frame.AddTransistion(symbol, valueFrame);
            }

            pool.ClearAndFree(transitions);
        }

        private static ISymbol GetPostDotSymbol(PreComputedState state)
        {
            return state.Production.RightHandSide[state.Position];
        }

        private static bool IsComplete(PreComputedState state)
        {
            return state.Position == state.Production.RightHandSide.Count;
        }
    }
}
