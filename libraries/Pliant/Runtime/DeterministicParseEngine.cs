using System;
using Pliant.Tokens;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Forest;
using System.Collections.Generic;
using Pliant.Utilities;
using System.Runtime.CompilerServices;

namespace Pliant.Runtime
{
    public class DeterministicParseEngine : IParseEngine
    {
        private readonly PreComputedGrammar _precomputedGrammar;
        private StateFrameChart _chart;

        public int Location { get; private set; }

        public IGrammar Grammar
        {
            get
            {
                return _precomputedGrammar.Grammar;
            }
        }

        public DeterministicParseEngine(IGrammar grammar)
            : this(new PreComputedGrammar(grammar))
        {
        }

        public DeterministicParseEngine(PreComputedGrammar preComputedGrammar)
        {
            _precomputedGrammar = preComputedGrammar;
            Initialize();
        }
        
        private void Initialize()
        {
            Location = 0;
            _chart = new StateFrameChart();
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
            var anyEarleySets = _chart.FrameSets.Count > 0;
            if (!anyEarleySets)
                return false;

            var lastFrameSetIndex = _chart.FrameSets.Count - 1;
            var lastFrameSet = _chart.FrameSets[lastFrameSetIndex];
            
            return AnyStateFrameAccepted(lastFrameSet);
        }

        private bool AnyStateFrameAccepted(StateFrameSet lastFrameSet)
        {
            var lastFrameSetFramesCount = lastFrameSet.Frames.Count;
            for (var i = 0; i < lastFrameSetFramesCount; i++)
            {
                var stateFrame = lastFrameSet.Frames[i];
                var originIsFirstEarleySet = stateFrame.Origin == 0;
                if (!originIsFirstEarleySet)
                    continue;

                if (AnyPreComputedStateAccepted(stateFrame.Frame.Data))
                    return true;
            }

            return false;
        }

        private bool AnyPreComputedStateAccepted(IReadOnlyList<IDottedRule> states)
        {
            for (var j = 0; j < states.Count; j++)
            {
                var preComputedState = states[j];
                if (!IsComplete(preComputedState))
                    continue;

                if (!IsStartState(preComputedState))
                    continue;

                return true;
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

            for (var d = 0; d < frame.Data.Count; ++d)
            {
                var preComputedState = frame.Data[d];

                var production = preComputedState.Production;
                
                if (!preComputedState.IsComplete)
                    continue;

                var leftHandSide = production.LeftHandSide;

                for (var p = 0; p < parentSetFramesCount; p++)
                {
                    var pState = parentSetFrames[p];
                    var pParent = pState.Origin;

                    Frame target = null;
                    if (!pState.Frame.Reductions.TryGetValue(leftHandSide, out target))
                        continue;

                    if (!_chart.Enqueue(i, new StateFrame(target, pParent)))
                        continue;

                    if (target.NullTransition == null)
                        continue;

                    _chart.Enqueue(i, new StateFrame(target.NullTransition, i));
                }
            }
        }

        private void Scan(int location, IToken token)
        {
            var set = _chart.FrameSets[location];
            var frames = set.Frames;
            var framesCount = frames.Count;
            
            for (var f = 0; f < framesCount; f++)
            {
                var stateFrame = frames[f];
                var parentOrigin = stateFrame.Origin;
                var frame = stateFrame.Frame;

                ScanFrame(location, token, parentOrigin, frame);
            }
        }

        private void ScanFrame(int location, IToken token, int parent, Frame frame)
        {
            Frame target;

            //PERF: This could perhaps be improved with an int array and direct index lookup based on "token.TokenType.Id"?...
            if (!frame.TokenTransitions.TryGetValue(token.TokenType, out target))
                return;

            if (!_chart.Enqueue(location + 1, new StateFrame(target, parent)))
                return;

            if (target.NullTransition == null)
                return;

            _chart.Enqueue(location + 1, new StateFrame(target.NullTransition, location + 1));
        }
        
        public void Reset()
        {
            _chart.Clear();
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            throw new NotImplementedException();
        }

        private Dictionary<int, IReadOnlyList<ILexerRule>> _expectedLexerRuleCache = new Dictionary<int, IReadOnlyList<ILexerRule>>();
        private static readonly ILexerRule[] EmptyLexerRules = { };

        public IReadOnlyList<ILexerRule> GetExpectedLexerRules()
        {
            if (_chart.FrameSets.Count == 0)
                return EmptyLexerRules;
            
            var frameSet = _chart.FrameSets[_chart.FrameSets.Count - 1];
            var hashCode = ComputeExpectedLexerRulesHashCode(frameSet);

            IReadOnlyList<ILexerRule> cachedLexerRules = null;

            if (_expectedLexerRuleCache.TryGetValue(hashCode, out cachedLexerRules))
                return cachedLexerRules;

            var listPool = SharedPools.Default<List<ILexerRule>>();
            List<ILexerRule> list = listPool.AllocateAndClear();

            for (var i = 0; i < frameSet.Frames.Count; i++)
            {
                var stateFrame = frameSet.Frames[i];
                for (int j = 0; j < stateFrame.Frame.ScanKeys.Count; j++)
                {
                    var lexerRule = stateFrame.Frame.ScanKeys[j];
                    list.Add(lexerRule);
                }
            }

            var array = list.ToArray();
            listPool.ClearAndFree(list);

            _expectedLexerRuleCache.Add(hashCode, array);

            return array;
        }

        private static int ComputeExpectedLexerRulesHashCode(StateFrameSet frameSet)
        {
            var hashCode = 0;
            var firstHash = true;
            for (var i = 0; i < frameSet.Frames.Count; i++)
            {
                var stateFrame = frameSet.Frames[i];
                for (int j = 0; j < stateFrame.Frame.ScanKeys.Count; j++)
                {
                    var lexerRule = stateFrame.Frame.ScanKeys[j];
                    hashCode = HashCode.ComputeIncrementalHash(lexerRule.GetHashCode(), hashCode, firstHash);
                    firstHash = false;
                }
            }

            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStartState(IDottedRule state)
        {
            var start = Grammar.Start;
            return state.Production.LeftHandSide.Equals(start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsComplete(IDottedRule preComputedState)
        {
            return preComputedState.Position == preComputedState.Production.RightHandSide.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ISymbol GetPostDotSymbol(IDottedRule preComputedState)
        {
            return preComputedState.Production.RightHandSide[preComputedState.Position];
        }
    }
}
