using Pliant.Grammars;
using System.Collections.Generic;
using Pliant.Utilities;
using Pliant.Collections;

namespace Pliant.Runtime
{
    public class PreComputedGrammar
    {
        private Dictionary<Frame, Frame> _frames;
        private Queue<Frame> _frameQueue;

        internal Frame Start { get; private set; }

        public IGrammar Grammar { get; private set; }

        public PreComputedGrammar(IGrammar grammar)
        {
            Grammar = grammar;
            _frames = new Dictionary<Frame, Frame>();
            _frameQueue = new Queue<Frame>(0);

            var startStates = new SortedSet<PreComputedState>();
            var startProductions = Grammar.StartProductions();
            for (int p = 0; p < startProductions.Count; p++)
            {
                var production = startProductions[p];
                var state = new PreComputedState(production, 0);
                startStates.Add(state);
            }
            
            // split will enqueue additional states
            // it returns the non lambda kernel state and the lambda kernel state
            // through the NullTransition property
            Start = Split(
                new Frame(startStates));

            while(_frameQueue.Count > 0)
                Split(_frameQueue.Dequeue());
        }

        private Frame Split(Frame frame)
        {
            var nonLambdaKernelStates = GetNonLambdaKernelStates(frame);
            var lambdaKernelStates = GetLambdaKernelStates(nonLambdaKernelStates);
            
            var nonLambdaKernelFrame = AddNewFrameOrGetExistingFrame(nonLambdaKernelStates);
            ProcessSymbolTransitions(nonLambdaKernelFrame);
            if (lambdaKernelStates.Count > 0)
            {
                var lambdaKernelFrame = AddNewFrameOrGetExistingFrame(lambdaKernelStates);
                nonLambdaKernelFrame.NullTransition = lambdaKernelFrame;
                ProcessSymbolTransitions(lambdaKernelFrame);
            }
            return nonLambdaKernelFrame;
        }        

        private SortedSet<PreComputedState> GetNonLambdaKernelStates(Frame frame)
        {
            var nonLambdaKernelStates = new SortedSet<PreComputedState>();

            foreach (var state in frame.Data)
            {
                nonLambdaKernelStates.Add(state);
                var production = state.Production;
                for (int s = state.Position; s < state.Production.RightHandSide.Count; s++)
                {
                    var postDotSymbol = production.RightHandSide[s];
                    if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                        break;
                    var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;
                    if (!Grammar.IsNullable(nonTerminalPostDotSymbol))
                        break;
                    nonLambdaKernelStates.Add(
                        new PreComputedState(production, s + 1));
                }
            }

            return nonLambdaKernelStates;
        }

        private SortedSet<PreComputedState> GetLambdaKernelStates(SortedSet<PreComputedState> nonLambdaKernelStates)
        {
            var kernelStates = new SortedSet<PreComputedState>();
            var queuePool = SharedPools.Default<Queue<PreComputedState>>();
            var queue = queuePool.AllocateAndClear();

            foreach (var nonKernelState in nonLambdaKernelStates)
                queue.Enqueue(nonKernelState);

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                var isComplete = state.Position == state.Production.RightHandSide.Count;
                if (isComplete)
                    continue;

                var postDotSymbol = state.Production.RightHandSide[state.Position];
                if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                    continue;

                var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;

                if (Grammar.IsNullable(nonTerminalPostDotSymbol))
                {
                    var aycockHorspoolState = new PreComputedState(state.Production, state.Position + 1);
                    if (!nonLambdaKernelStates.Contains(aycockHorspoolState))
                        if (kernelStates.Add(aycockHorspoolState))
                            queue.Enqueue(aycockHorspoolState);
                }

                var predictedProductions = Grammar.RulesFor(nonTerminalPostDotSymbol);

                for (int p = 0; p < predictedProductions.Count; p++)
                {
                    var production = predictedProductions[p];
                    var predictedState = new PreComputedState(production, 0);
                    if (!nonLambdaKernelStates.Contains(predictedState))
                        if (kernelStates.Add(predictedState))
                            queue.Enqueue(predictedState);
                }
            }

            queuePool.ClearAndFree(queue);

            return kernelStates;
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
                var keyFrame = new Frame(transitions[symbol]);
                Frame valueFrame = null;
                if (!_frames.TryGetValue(keyFrame, out valueFrame))
                {
                    valueFrame = keyFrame;
                    _frames[keyFrame] = valueFrame;
                    _frameQueue.Enqueue(valueFrame);
                }
                frame.AddTransistion(symbol, valueFrame);
            }

            pool.ClearAndFree(transitions);
        }

        
    }    

    
}
