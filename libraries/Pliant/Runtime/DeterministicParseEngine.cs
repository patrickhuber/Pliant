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
            for (int f = 0; f < set.Frames.Count; f++)
            {
                var state = set.Frames[f];
                var parent = state.Origin;
                var frame = state.Frame;

                if (parent == i)
                    continue;

                ReduceFrame(i, parent, frame);
            }
        }

        private void ReduceFrame(int i, int parent, Frame frame)
        {
            var parentSet = _chart.FrameSets[parent];
            var parentSetFrames = parentSet.Frames;
            var parentSetFramesCount = parentSetFrames.Count;

            for (int d = 0; d < frame.Data.Length; ++d)
            {
                var preComputedState = frame.Data[d];

                var production = preComputedState.Production;

                var isComplete = preComputedState.Position == production.RightHandSide.Count;
                if (!isComplete)
                    continue;

                var leftHandSide = production.LeftHandSide;

                for (int p = 0; p < parentSetFramesCount; p++)
                {
                    var pState = parentSetFrames[p];
                    var pParent = pState.Origin;

                    Frame target = null;
                    if (!pState.Frame.Transitions.TryGetValue(leftHandSide, out target))
                        continue;

                    if (!_chart.Enqueue(i, new StateFrame(target, pParent)))
                        continue;

                    if (target.NullTransition == null)
                        continue;

                    _chart.Enqueue(i, new StateFrame(target.NullTransition, i));
                }
            }
        }

        private void Scan(int i, IToken token)
        {
            var set = _chart.FrameSets[i];
            var frames = set.Frames;
            var framesCount = frames.Count;
            
            for (var f = 0; f < framesCount; f++)
            {
                var stateFrame = frames[f];
                var parentOrigin = stateFrame.Origin;
                var frame = stateFrame.Frame;

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
