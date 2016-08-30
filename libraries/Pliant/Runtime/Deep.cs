using Pliant.Grammars;
using System.Collections.Generic;
using Pliant.Utilities;
using Pliant.Collections;

namespace Pliant.Runtime
{
    public class Deep
    {
        private IGrammar _grammar;
        private Dictionary<Frame, Frame> _frames;
        private Queue<Frame> _frameQueue;

        public Deep(IGrammar grammar)
        {
            _grammar = grammar;
            _frames = new Dictionary<Frame, Frame>();
            _frameQueue = new Queue<Frame>(0);

            var startStates = new SortedSet<PreComputedState>();
            var startProductions = _grammar.StartProductions();
            for (int p = 0; p < startProductions.Count; p++)
            {
                var production = startProductions[p];
                var state = new PreComputedState(production, 0);
                startStates.Add(state);
            }

            var startFrame = new Frame(startStates);
            _frameQueue.Enqueue(startFrame);

            while(_frameQueue.Count > 0)
                ProcessFrame(_frameQueue.Dequeue());
        }

        private void ProcessFrame(Frame frame)
        {
            SortedSet<PreComputedState> nonLambdaKernelStates = null;
            SortedSet<PreComputedState> lambdaKernelStates = null;

            Split(frame, out nonLambdaKernelStates, out lambdaKernelStates);

            var nonLambdaKernelFrame = AddNewFrameOrGetExistingFrame(nonLambdaKernelStates);
            ProcessSymbolTransitions(nonLambdaKernelFrame);
            if (lambdaKernelStates.Count > 0)
            {
                var lambdaKernelFrame = AddNewFrameOrGetExistingFrame(lambdaKernelStates);
                nonLambdaKernelFrame.NullTransition = lambdaKernelFrame;
                ProcessSymbolTransitions(lambdaKernelFrame);
            }
        }

        private void Split(
            Frame frame, 
            out SortedSet<PreComputedState> nonLambdaKernelStates, 
            out SortedSet<PreComputedState> lambdaKernelStates)
        {
            nonLambdaKernelStates = GetNonLambdaKernelStates(frame);
            lambdaKernelStates = GetLambdaKernelStates(nonLambdaKernelStates);
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
                    if (!_grammar.IsNullable(nonTerminalPostDotSymbol))
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

                if (_grammar.IsNullable(nonTerminalPostDotSymbol))
                {
                    var aycockHorspoolState = new PreComputedState(state.Production, state.Position + 1);
                    if (!nonLambdaKernelStates.Contains(aycockHorspoolState))
                        if (kernelStates.Add(aycockHorspoolState))
                            queue.Enqueue(aycockHorspoolState);
                }

                var predictedProductions = _grammar.RulesFor(nonTerminalPostDotSymbol);

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

        private class Frame
        {
            public SortedSet<PreComputedState> Data { get; private set; }
            public Dictionary<ISymbol, Frame> Transitions { get; private set; }
            public Frame NullTransition { get; set; }

            public Frame(SortedSet<PreComputedState> data)
            {
                Data = data;
                Transitions = new Dictionary<ISymbol, Frame>();
                _hashCode = ComputeHashCode(data);
            }

            private readonly int _hashCode;

            public void AddTransistion(ISymbol symbol, Frame target)
            {
                Frame value = null;
                if (!Transitions.TryGetValue(symbol, out value))
                    Transitions.Add(symbol, target);
            }

            static int ComputeHashCode(SortedSet<PreComputedState> data)
            {
                return HashCode.Compute(data);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                if (((object)obj) == null)
                    return false;

                var frame = obj as Frame;
                if (((object)frame) == null)
                    return false;

                foreach (var item in Data)
                    if (!frame.Data.Contains(item))
                        return false;

                return true;
            }
        }
    }    

    
}
