using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Collections;
using System.Collections.Generic;
using Pliant.Utilities;
using Pliant.Tokens;

namespace Pliant.Runtime
{
    public class MarpaParseEngine 
    {
        private PreComputedGrammar _preComputedGrammar;

        public StateFrameChart Chart { get; private set; }
        
        public int Location { get; private set; }

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
                    if (!IsCompleted(dottedRule))
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
                for (var j = 0; j < stateFrame.Frame.Data.Count; j++)
                {
                    var preComputedState = stateFrame.Frame.Data[j];
                    if (IsCompleted(preComputedState))
                        continue;
                    var postDotSymbol = GetPostDotSymbol(preComputedState);

                    // leo eligibile items are right recursive directly or indirectly                    
                    if (!_preComputedGrammar.IsRightRecursive(postDotSymbol))
                        continue;

                    // to determine if the item is leo unique, cache it here
                    var count = 0;
                    if (!cachedCount.TryGetValue(postDotSymbol, out count))
                    {
                        cachedCount[postDotSymbol] = 1;

                        var origin = stateFrame.Origin;
                        CachedStateFrameTransition topCacheItem = null;
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
                                                
                        cachedTransitions[postDotSymbol] = new CachedStateFrameTransition(
                            postDotSymbol,
                            stateFrame.Frame,
                            topCacheItem == null ? stateFrame.Origin : origin);
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
                    return;
                frameSet.AddCachedTransition(cachedTransitions[symbol]);
            }

            cachedTransitionsPool.ClearAndFree(cachedTransitions);
            cachedCountPool.ClearAndFree(cachedCount);
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

        private static bool IsCompleted(PreComputedState dottedRule)
        {
            return dottedRule.Production.RightHandSide.Count == dottedRule.Position;
        }

        private static ISymbol GetPostDotSymbol(PreComputedState preComputedState)
        {
            return preComputedState.Production.RightHandSide[preComputedState.Position];
        }

        private bool IsLeoEligible(ISymbol symbol, StateFrameSet stateFrameSet)
        {
            return stateFrameSet.IsLeoUnique(symbol)
                && _preComputedGrammar.IsRightRecursive(symbol);
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
    }
}
