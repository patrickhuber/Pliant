using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Collections;
using System.Collections.Generic;
using Pliant.Utilities;
using Pliant.Tokens;
using System;
using System.Runtime.CompilerServices;
using Pliant.Forest;

namespace Pliant.Runtime
{
    public class MarpaParseEngine : IParseEngine
    {
        private PreComputedGrammar _preComputedGrammar;

        public StateFrameChart Chart { get; private set; }
        
        public int Location { get; private set; }

        public IGrammar Grammar => _preComputedGrammar.Grammar;

        public MarpaParseEngine(PreComputedGrammar preComputedGrammar)
        {
            _preComputedGrammar = preComputedGrammar;
            Chart = new StateFrameChart();
            Initialize();
        }

        public MarpaParseEngine(IGrammar grammar)
            : this(new PreComputedGrammar(grammar))
        { }

        private void Initialize()
        {
            var start = _preComputedGrammar.Start;
            AddEimPair(0, start, 0);
        }

        public void Reset()
        {
            Initialize();
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            throw new NotImplementedException();
        }

        private Dictionary<int, IReadOnlyList<ILexerRule>> _expectedLexerRuleCache = new Dictionary<int, IReadOnlyList<ILexerRule>>();
        private static readonly ILexerRule[] EmptyLexerRules = { };

        public IReadOnlyList<ILexerRule> GetExpectedLexerRules()
        {
            var frameSets = Chart.FrameSets;
            var frameSetCount = frameSets.Count;

            if (frameSetCount == 0)
                return EmptyLexerRules;
                        
            var frameSet = frameSets[frameSetCount - 1];
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
            var count = 0;
            for (var i = 0; i < frameSet.Frames.Count; i++)
            {
                var stateFrame = frameSet.Frames[i];
                for (int j = 0; j < stateFrame.Frame.ScanKeys.Count; j++)
                {
                    var lexerRule = stateFrame.Frame.ScanKeys[j];
                    hashCode = HashCode.ComputeIncrementalHash(lexerRule.GetHashCode(), hashCode, count == 0);
                    count++;
                }
            }
            
            return hashCode;
        }

        public bool Pulse(IToken token)
        {
            ScanPasss(Location, token);
            var tokenRecognized = Chart.FrameSets.Count > Location + 1;
            if (!tokenRecognized)
                return false;
            Location++;
            ReductionPass(Location);
            return true;
        }

        public bool IsAccepted()
        {
            var anyEarleySets = Chart.FrameSets.Count > 0;
            if (!anyEarleySets)
                return false;

            var lastFrameSetIndex = Chart.FrameSets.Count - 1;
            var lastFrameSet = Chart.FrameSets[lastFrameSetIndex];

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
                var isCompleted = preComputedState.Position == preComputedState.Production.RightHandSide.Count;
                if (!preComputedState.IsComplete)
                    continue;

                if (!IsStartState(preComputedState))
                    continue;

                return true;
            }
            return false;
        }

        private void ScanPasss(int iLoc, IToken token)
        {
            var iES = Chart.FrameSets[iLoc];
            for (var i = 0; i < iES.Frames.Count; i++)
            {
                var workEIM = iES.Frames[i];
                var fromAH = workEIM.Frame;
                var origLoc = workEIM.Origin;

                var toAH = Goto(fromAH, token);
                if (toAH == null)
                    continue;
                AddEimPair(iLoc + 1, toAH, origLoc);
            }
        }

        private void ReductionPass(int iLoc)
        {
            var iES = Chart.FrameSets[iLoc];
            var processed = SharedPools.Default<HashSet<ISymbol>>().AllocateAndClear();
            for (var i = 0; i < iES.Frames.Count; i++)
            {
                var workEIM = iES.Frames[i];
                var workAH = workEIM.Frame;
                var origLoc = workEIM.Origin;

                for (var j = 0; j < workAH.Data.Count; j++)
                {
                    var dottedRule = workAH.Data[j];
                    if (!dottedRule.IsComplete)
                        continue;

                    var lhsSym = dottedRule.Production.LeftHandSide;
                    if (!processed.Add(lhsSym))
                        continue;

                    ReduceOneLeftHandSide(iLoc, origLoc, lhsSym);
                }
                processed.Clear();
            }
            SharedPools.Default<HashSet<ISymbol>>().ClearAndFree(processed);
            MemoizeTransitions(iLoc);
        }

        private void ReduceOneLeftHandSide(int iLoc, int origLoc, INonTerminal lhsSym)
        {
            var frameSet = Chart.FrameSets[origLoc];
            var transitionItem = frameSet.FindCachedStateFrameTransition(lhsSym);
            if (transitionItem != null)
                LeoReductionOperation(iLoc, transitionItem);
            else
            {
                for (var i = 0; i < frameSet.Frames.Count; i++)
                {
                    var stateFrame = frameSet.Frames[i];
                    EarleyReductionOperation(iLoc, stateFrame, lhsSym);
                }
            }
        }
        
        private void MemoizeTransitions(int iLoc)
        {
            var frameSet = Chart.FrameSets[iLoc];
            // leo eligibility needs to be cached before creating the cached transition
            // if the size of the list is != 1, do not enter the cached frame transition
            var cachedTransitionsPool = SharedPools.Default<Dictionary<ISymbol, CachedStateFrameTransition>>();
            var cachedTransitions = cachedTransitionsPool.AllocateAndClear();
            var cachedCountPool = SharedPools.Default<Dictionary<ISymbol, int>>();
            var cachedCount = cachedCountPool.AllocateAndClear();

            for (var i = 0; i < frameSet.Frames.Count; i++)
            {
                var stateFrame = frameSet.Frames[i];
                var frame = stateFrame.Frame;
                var frameData = frame.Data;
                var stateFrameDataCount = frameData.Count;

                for (var j = 0; j < stateFrameDataCount; j++)
                {
                    var preComputedState = frameData[j];
                    if (preComputedState.IsComplete)
                        continue;

                    var postDotSymbol = preComputedState.PostDotSymbol;
                    if (postDotSymbol.SymbolType != SymbolType.NonTerminal)
                        continue;

                    // leo eligibile items are right recursive directly or indirectly                    
                    if (!_preComputedGrammar.Grammar.IsRightRecursive(
                        preComputedState.Production.LeftHandSide))
                        continue;

                    // to determine if the item is leo unique, cache it here
                    var count = 0;
                    if (!cachedCount.TryGetValue(postDotSymbol, out count))
                    {
                        cachedCount[postDotSymbol] = 1;
                        cachedTransitions[postDotSymbol] = CreateTopCachedItem(stateFrame, postDotSymbol);
                    }
                    else
                    {
                        cachedCount[postDotSymbol] = count + 1;
                    }
                }
            }

            // add all memoized leo items to the frameSet
            foreach (var symbol in cachedCount.Keys)
            {
                var count = cachedCount[symbol];
                if (count != 1)
                    continue;
                frameSet.AddCachedTransition(cachedTransitions[symbol]);
            }

            cachedTransitionsPool.ClearAndFree(cachedTransitions);
            cachedCountPool.ClearAndFree(cachedCount);
        }

        private CachedStateFrameTransition CreateTopCachedItem(
            StateFrame stateFrame, 
            ISymbol postDotSymbol)
        {
            var origin = stateFrame.Origin;
            CachedStateFrameTransition topCacheItem = null;
            // search for the top item in the leo chain
            while (true)
            {
                var originFrameSet = Chart.FrameSets[stateFrame.Origin];
                var nextCachedItem = originFrameSet.FindCachedStateFrameTransition(postDotSymbol);
                if (nextCachedItem == null)
                    break;
                topCacheItem = nextCachedItem;
                if (origin == nextCachedItem.Origin)
                    break;
                origin = topCacheItem.Origin;
            }

            return new CachedStateFrameTransition(
                postDotSymbol,
                stateFrame.Frame,
                topCacheItem == null ? stateFrame.Origin : origin);
        }

        private void EarleyReductionOperation(int iLoc, StateFrame fromEim, ISymbol transSym)
        {
            var fromAH = fromEim.Frame;
            var originLoc = fromEim.Origin;

            var toAH = Goto(fromAH, transSym);
            if (toAH == null)
                return;

            AddEimPair(iLoc, toAH, originLoc);
        }

        private void LeoReductionOperation(int iLoc, CachedStateFrameTransition fromLim)
        {
            var fromAH = fromLim.Frame;
            var transSym = fromLim.Symbol;
            var originLoc = fromLim.Origin;

            var toAH = Goto(fromAH, transSym);
            if (toAH == null)
                return;

            AddEimPair(iLoc, toAH, originLoc);
        }

        private void AddEimPair(int iLoc, Frame confirmedAH, int origLoc)
        {
            var confirmedEIM = new StateFrame(confirmedAH, origLoc);
            var predictedAH = Goto(confirmedAH);
            Chart.Enqueue(iLoc, confirmedEIM);
            if (predictedAH == null)
                return;
            var predictedEIM = new StateFrame(predictedAH, iLoc);
            Chart.Enqueue(iLoc, predictedEIM);
        }
                
        private static Frame Goto(Frame fromAH)
        {
            return fromAH.NullTransition;
        }

        private static Frame Goto(Frame fromAH, ISymbol symbol)
        {
            return fromAH.Reductions.GetOrReturnNull(symbol);         
        }

        private static Frame Goto(Frame fromAH, IToken token)
        {
            return fromAH.TokenTransitions.GetOrReturnNull(token.TokenType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsStartState(IDottedRule state)
        {
            var start = _preComputedGrammar.Grammar.Start;
            return state.Production.LeftHandSide.Equals(start);
        }
    }
}
