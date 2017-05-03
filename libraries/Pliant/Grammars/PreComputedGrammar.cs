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
        
        public PreComputedGrammar(IGrammar grammar)
        {
            _frameQueue = new ProcessOnceQueue<Frame>();
            _frames = new Dictionary<Frame, Frame>();

            Grammar = grammar;
            
            var startStates = Initialize(Grammar);
            Start = AddNewFrameOrGetExistingFrame(startStates);
            ProcessFrameQueue();
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

        private SortedSet<IDottedRule> GetPredictedStates(Frame frame)
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

        private Frame AddNewFrameOrGetExistingFrame(SortedSet<IDottedRule> states)
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

        private bool TryGetExistingFrameOrCreateNew(SortedSet<IDottedRule> states, out Frame outFrame)
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
                var valueFrame = AddNewFrameOrGetExistingFrame(confirmedStates);
                frame.AddTransistion(symbol, valueFrame);
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
