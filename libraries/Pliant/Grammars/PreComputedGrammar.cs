using Pliant.Collections;
using System.Collections.Generic;
using System;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class PreComputedGrammar
    {
        public IGrammar Grammar { get; private set; }

        internal Frame Start { get; private set; }

        private ProcessOnceQueue<Frame> _frameQueue;

        private Dictionary<Frame, Frame> _frames;

        public PreComputedGrammar(IGrammar grammar)
        {
            _frameQueue = new ProcessOnceQueue<Frame>();
            _frames = new Dictionary<Frame, Frame>();

            Grammar = grammar;
            var startStates = Initialize(Grammar);
            Start = AddNewFrameOrGetExistingFrame(startStates);

            while (_frameQueue.Count > 0)
            {
                // assume the closure has already been captured
                var frame = _frameQueue.Dequeue();
                ProcessSymbolTransitions(frame);

                // capture the predictions for the frame
                var nonLambdaKernelStates = GetLambdaKernelStates(frame.Data);

                // if no predictions, continue
                if (nonLambdaKernelStates.Count == 0)
                    continue;

                // assign the null transition
                // only process symbols on the null frame if it is new
                Frame nullFrame;
                if (!TryGetExistingFrameOrCreateNew(nonLambdaKernelStates, out nullFrame))
                    ProcessSymbolTransitions(nullFrame);

                frame.NullTransition = nullFrame;
            }
        }

        private SortedSet<PreComputedState> Initialize(IGrammar grammar)
        {
            var pool = SharedPools.Default<SortedSet<PreComputedState>>();
            
            var startStates = pool.AllocateAndClear();
            var startProductions = grammar.StartProductions();

            for (int p = 0; p < startProductions.Count; p++)
            {
                var production = startProductions[p];
                var state = new PreComputedState(production, 0);
                startStates.Add(state);
            }

            var closure = GetNonLambdaKernelStates(startStates);

            pool.ClearAndFree(startStates);
            return closure;
        }

        private SortedSet<PreComputedState> GetNonLambdaKernelStates(SortedSet<PreComputedState> states)
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
                for (int s = state.Position; s < state.Production.RightHandSide.Count; s++)
                {
                    var postDotSymbol = production.RightHandSide[s];
                    if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                        break;

                    var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;
                    if (!Grammar.IsNullable(nonTerminalPostDotSymbol))
                        break;

                    var preComputedState = new PreComputedState(production, s + 1);
                    if (closure.Add(preComputedState))
                        queue.Enqueue(preComputedState);
                }
            }
            pool.ClearAndFree(queue);
            return closure;
        }

        private SortedSet<PreComputedState> GetLambdaKernelStates(SortedSet<PreComputedState> states)
        {
            var pool = SharedPools.Default<Queue<PreComputedState>>();

            var queue = pool.AllocateAndClear();
            var closure = new SortedSet<PreComputedState>();

            foreach (var state in states)
                if (!IsComplete(state))
                    queue.Enqueue(state);

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (IsComplete(state))
                    continue;

                var production = state.Production;

                var postDotSymbol = production.RightHandSide[state.Position];
                if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                    continue;

                var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;
                if (Grammar.IsNullable(nonTerminalPostDotSymbol))
                {
                    var preComputedState = new PreComputedState(production, state.Position + 1);
                    if (!states.Contains(preComputedState))
                        if (closure.Add(preComputedState))
                            if (!IsComplete(preComputedState))
                                queue.Enqueue(preComputedState);
                }

                var predictions = Grammar.RulesFor(nonTerminalPostDotSymbol);
                for (var p = 0; p < predictions.Count; p++)
                {
                    var prediction = predictions[p];
                    var preComputedState = new PreComputedState(prediction, 0);
                    if (!states.Contains(preComputedState))
                        if (closure.Add(preComputedState))
                            if (!IsComplete(preComputedState))
                                queue.Enqueue(preComputedState);
                }
            }

            pool.ClearAndFree(queue);
            return closure;
        }

        private static bool IsComplete(PreComputedState state)
        {
            return state.Position == state.Production.RightHandSide.Count;
        }

        private Frame AddNewFrameOrGetExistingFrame(SortedSet<PreComputedState> states)
        {
            var frame = new Frame(states);
            Frame outFrame = null;
            if (!_frames.TryGetValue(frame, out outFrame))
            {
                outFrame = frame;
                _frames[frame] = outFrame;
                _frameQueue.Enqueue(outFrame);
            }
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

            foreach (var nfaState in frame.Data)
            {
                var isCompleted = nfaState.Position == nfaState.Production.RightHandSide.Count;
                if (isCompleted)
                    continue;

                var production = nfaState.Production;
                var postDotSymbol = production.RightHandSide[nfaState.Position];

                var targetStates = transitions.AddOrGetExisting(postDotSymbol);

                var nextRule = new PreComputedState(production, nfaState.Position + 1);
                targetStates.Add(nextRule);
            }

            foreach (var symbol in transitions.Keys)
            {
                var closure = GetNonLambdaKernelStates(transitions[symbol]);
                var valueFrame = AddNewFrameOrGetExistingFrame(closure);
                frame.AddTransistion(symbol, valueFrame);
            }

            pool.ClearAndFree(transitions);
        }
    }
}
