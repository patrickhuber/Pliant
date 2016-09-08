using System;
using Pliant.Tokens;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Forest;
using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class DeterministicParseEngine : IParseEngine
    {
        PreComputedGrammar _precomputedGrammar;
        PreComputedChart _chart;

        public int Location { get; private set; }

        public IGrammar Grammar
        {
            get
            {
                return _precomputedGrammar.Grammar;
            }
        }

        public DeterministicParseEngine(PreComputedGrammar preComputedGrammar)
        {
            _precomputedGrammar = preComputedGrammar;
            Initialize();
        }

        private void Initialize()
        {
            Location = 0;
            _chart = new PreComputedChart();
            var kernelFrame = _precomputedGrammar.Start;
            Enqueue(Location, new StateFrame(kernelFrame, 0));
            Reduce(Location);
        }

        private bool Enqueue(int location, StateFrame stateFrame)
        {
            if (!_chart.Enqueue(location, stateFrame))
                return false;

            if (stateFrame.Frame.NullTransition == null)
                return true;

            var nullTransitionFrame = new StateFrame(
                stateFrame.Frame.NullTransition,
                location);

            return _chart.Enqueue(location, nullTransitionFrame);
        }

        public bool Pulse(IToken token)
        {
            Scan(Location, token);
            var tokenRecognized = _chart.FrameSets.Count > Location + 1;
            if (!tokenRecognized)
                return false;
            Location++;
            Reduce(Location);
            return true;
        }

        public bool IsAccepted()
        {
            if (_chart.FrameSets.Count == 0)
                return false;

            var lastFrameSetIndex = _chart.FrameSets.Count - 1;
            var lastFrameSet = _chart.FrameSets[lastFrameSetIndex];

            var start = Grammar.Start;

            for (var i = 0; i < lastFrameSet.Frames.Count; i++)
            {
                var stateFrame = lastFrameSet.Frames[i];
                if (stateFrame.Origin != 0)
                    continue;

                foreach (var preComputedState in stateFrame.Frame.Data)
                {
                    var isCompleted = preComputedState.Position == preComputedState.Production.RightHandSide.Count;
                    if (!isCompleted)
                        continue;

                    var isStartState = preComputedState.Production.LeftHandSide.Equals(start);
                    if (!isStartState)
                        continue;

                    return true;
                }
            }

            return false;
        }

        private void Reduce(int i)
        {
            var set = _chart.FrameSets[i];
            var setFrames = set.Frames;
            var framesCount = setFrames.Count;

            //PERF: not sure if it helps moving decl outside of loop
            int parentOrigin;
            Frame frame;
            StateFrame stateFrame;
            bool hasChanged;
            var f = 0;

            for (; f < framesCount; f++)
            {
                stateFrame = setFrames[f];
                parentOrigin = stateFrame.Origin;
                frame = stateFrame.Frame;

                if (parentOrigin == i)
                    continue;

                hasChanged = ReduceFrame(i, parentOrigin, frame);
                if (hasChanged)
                {
                    setFrames = set.Frames;
                    framesCount = setFrames.Count;
                }
            }
        }

        private bool ReduceFrame(int i, int parent, Frame frame)
        {
            var hasChanged = false;
            
            var frameData = frame.DataPerf;
            var frameDataCount = frameData.Length;

            var parentSet = _chart.FrameSets[parent];
            var parentSetFrames = parentSet.FramesPerf;
            var parentSetFramesCount = parentSetFrames.Length;
            StateFrame pState;
            int pParent;
            Frame target;
            Frame nullTransition;

            PreComputedState preComputedState;
            IProduction preComputedStateProduction;
            INonTerminal leftHandSide;
            IReadOnlyList<ISymbol> productionRhs;
            StateFrame newStateFrame;

            int d = 0;
            int p = 0;

            for (; d < frameDataCount; ++d)
            {
                preComputedState = frameData[d];
                preComputedStateProduction = preComputedState.Production;
                productionRhs = preComputedStateProduction.RightHandSide;

                var isComplete = preComputedState.Position == productionRhs.Count;
                if (!isComplete)
                    continue;

                leftHandSide = preComputedStateProduction.LeftHandSide;

                p = 0;
                for (; p < parentSetFramesCount; p++)
                {
                    pState = parentSetFrames[p];
                    pParent = pState.Origin;

                    if (!pState.Frame.Transitions.TryGetValue(leftHandSide, out target))
                        continue;

                    newStateFrame = new StateFrame(target, pParent);

                    if (!_chart.Enqueue(i, newStateFrame))
                        continue;

                    hasChanged = true;

                    nullTransition = target.NullTransition;

                    if (nullTransition == null)
                        continue;

                    newStateFrame = new StateFrame(nullTransition, i);

                    _chart.Enqueue(i, newStateFrame);
                }
            }

            return hasChanged;
        }

        private void Scan(int i, IToken token)
        {
            var set = _chart.FrameSets[i];
            var frames = set.FramesPerf;
            var framesCount = frames.Length;

            //PERF: not sure if it helps moving decl outside of loop
            int parentOrigin;
            Frame frame;
            StateFrame stateFrame;
            var f = 0;
            for (; f < framesCount; f++)
            {
                stateFrame = frames[f];
                parentOrigin = stateFrame.Origin;
                frame = stateFrame.Frame;

                ScanFrame(i, token, parentOrigin, frame);
            }
        }

        private void ScanFrame(int i, IToken token, int parent, Frame frame)
        {
            Frame target;

            //PERF: This could perhaps be improved with an int array and direct index lookup based on "token.TokenType.Id"?...
            if (!frame.TokenTransitions.TryGetValue(token.TokenType, out target))
                return;

            if (!_chart.Enqueue(i + 1, new StateFrame(target, parent)))
                return;

            if (target.NullTransition == null)
                return;

            _chart.Enqueue(i + 1, new StateFrame(target.NullTransition, i + 1));
        }

        public void Reset()
        {
            _chart.Clear();
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            throw new NotImplementedException();
        }

        public List<ILexerRule> GetExpectedLexerRules()
        {
            var list = SharedPools.Default<List<ILexerRule>>().AllocateAndClear();

            if (_chart.FrameSets.Count == 0)
                return list;

            var frameSet = _chart.FrameSets[_chart.FrameSets.Count - 1];
            for (int i = 0; i < frameSet.Frames.Count; i++)
            {
                var stateFrame = frameSet.Frames[i];
                foreach (var lexerRule in stateFrame.Frame.Scans.Keys)
                    list.Add(lexerRule);
            }
            return list;
        }
    }
}
