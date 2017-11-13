using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Collections;
using System.Collections.Generic;
using Pliant.Utilities;
using Pliant.Tokens;
using System;
using System.Runtime.CompilerServices;
using Pliant.Forest;
using System.Collections;

namespace Pliant.Runtime
{
    public class MarpaParseEngine : IParseEngine
    {
        private PreComputedGrammar _preComputedGrammar;

        public DeterministicChart Chart { get; private set; }
        
        public int Location { get; private set; }

        public IGrammar Grammar => _preComputedGrammar.Grammar;

        public MarpaParseEngine(PreComputedGrammar preComputedGrammar)
        {
            _preComputedGrammar = preComputedGrammar;
            Chart = new DeterministicChart();
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
        
        private Dictionary<int, ILexerRule[]> _expectedLexerRuleCache;
        private static readonly ILexerRule[] EmptyLexerRules = { };
        private BitArray _expectedLexerRuleIndicies;

        public IReadOnlyList<ILexerRule> GetExpectedLexerRules()
        {
            var frameSets = Chart.Sets;
            var frameSetCount = frameSets.Count;

            if (frameSetCount == 0)
                return EmptyLexerRules;

            var hashCode = 0;
            var count = 0;

            if (_expectedLexerRuleIndicies == null)
                _expectedLexerRuleIndicies = new BitArray(Grammar.LexerRules.Count);
            else
                _expectedLexerRuleIndicies.SetAll(false);

            var frameSet = frameSets[frameSets.Count - 1];
            for (var i = 0; i < frameSet.States.Count; i++)
            {
                var stateFrame = frameSet.States[i];
                for (int j = 0; j < stateFrame.DottedRuleSet.ScanKeys.Count; j++)
                {
                    var lexerRule = stateFrame.DottedRuleSet.ScanKeys[j];
                    var index = Grammar.GetLexerRuleIndex(lexerRule);
                    if (index < 0)
                        continue;
                    if (_expectedLexerRuleIndicies[index])
                        continue;

                    _expectedLexerRuleIndicies[index] = true;
                    hashCode = HashCode.ComputeIncrementalHash(lexerRule.GetHashCode(), hashCode, count == 0);
                    count++;
                }
            }

            if (_expectedLexerRuleCache == null)
                _expectedLexerRuleCache = new Dictionary<int, ILexerRule[]>();

            // if the hash is found in the cached lexer rule lists, return the cached array
            ILexerRule[] cachedLexerRules = null;
            if (_expectedLexerRuleCache.TryGetValue(hashCode, out cachedLexerRules))
            {
                return cachedLexerRules;
            }

            // compute the new lexer rule array and add it to the cache
            var array = new ILexerRule[count];
            var returnItemIndex = 0;
            for (var i = 0; i < Grammar.LexerRules.Count; i++)
                if (_expectedLexerRuleIndicies[i])
                {
                    array[returnItemIndex] = Grammar.LexerRules[i];
                    returnItemIndex++;
                }
            
            _expectedLexerRuleCache.Add(hashCode, array);

            return array;
        }
        
        public bool Pulse(IToken token)
        {
            ScanPass(Location, token);
            var tokenRecognized = Chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
                return false;
            Location++;
            ReductionPass(Location);
            return true;
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
                ScanPass(Location, tokens[i]);
            var tokenRecognized = Chart.Sets.Count > Location + 1;
            if (!tokenRecognized)
                return false;
            Location++;
            ReductionPass(Location);
            return true;
        }

        public bool IsAccepted()
        {
            var anyEarleySets = Chart.Sets.Count > 0;
            if (!anyEarleySets)
                return false;

            var lastDeterministicSetIndex = Chart.Sets.Count - 1;
            var lastDeterministicSet = Chart.Sets[lastDeterministicSetIndex];

            return AnyDeterministicStateAccepted(lastDeterministicSet);
        }

        private bool AnyDeterministicStateAccepted(DeterministicSet lastFrameSet)
        {
            var lastDeterministicStateCount = lastFrameSet.States.Count;
            for (var i = 0; i < lastDeterministicStateCount; i++)
            {
                var deterministicState = lastFrameSet.States[i];
                var originIsFirstEarleySet = deterministicState.Origin == 0;
                if (!originIsFirstEarleySet)
                    continue;

                if (AnyPreComputedStateAccepted(deterministicState.DottedRuleSet.Data))
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

        private void ScanPass(int iLoc, IToken token)
        {
            var iES = Chart.Sets[iLoc];
            for (var i = 0; i < iES.States.Count; i++)
            {
                var workEIM = iES.States[i];
                var fromAH = workEIM.DottedRuleSet;
                var origLoc = workEIM.Origin;

                var toAH = Goto(fromAH, token);
                if (toAH == null)
                    continue;
                AddEimPair(iLoc + 1, toAH, origLoc);
            }
        }

        private void ReductionPass(int iLoc)
        {
            var iES = Chart.Sets[iLoc];
            var processed = SharedPools.Default<HashSet<ISymbol>>().AllocateAndClear();
            for (var i = 0; i < iES.States.Count; i++)
            {
                var workEIM = iES.States[i];
                var workAH = workEIM.DottedRuleSet;
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
            var frameSet = Chart.Sets[origLoc];
            var transitionItem = frameSet.FindCachedDottedRuleSetTransition(lhsSym);
            if (transitionItem != null)
                LeoReductionOperation(iLoc, transitionItem);
            else
            {
                for (var i = 0; i < frameSet.States.Count; i++)
                {
                    var stateFrame = frameSet.States[i];
                    EarleyReductionOperation(iLoc, stateFrame, lhsSym);
                }
            }
        }
        
        private void MemoizeTransitions(int iLoc)
        {
            var frameSet = Chart.Sets[iLoc];
            // leo eligibility needs to be cached before creating the cached transition
            // if the size of the list is != 1, do not enter the cached frame transition
            var cachedTransitionsPool = SharedPools.Default<Dictionary<ISymbol, CachedDottedRuleSetTransition>>();
            var cachedTransitions = cachedTransitionsPool.AllocateAndClear();
            var cachedCountPool = SharedPools.Default<Dictionary<ISymbol, int>>();
            var cachedCount = cachedCountPool.AllocateAndClear();

            for (var i = 0; i < frameSet.States.Count; i++)
            {
                var stateFrame = frameSet.States[i];
                var frame = stateFrame.DottedRuleSet;
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

        private CachedDottedRuleSetTransition CreateTopCachedItem(
            DeterministicState stateFrame, 
            ISymbol postDotSymbol)
        {
            var origin = stateFrame.Origin;
            CachedDottedRuleSetTransition topCacheItem = null;
            // search for the top item in the leo chain
            while (true)
            {
                var originFrameSet = Chart.Sets[stateFrame.Origin];
                var nextCachedItem = originFrameSet.FindCachedDottedRuleSetTransition(postDotSymbol);
                if (nextCachedItem == null)
                    break;
                topCacheItem = nextCachedItem;
                if (origin == nextCachedItem.Origin)
                    break;
                origin = topCacheItem.Origin;
            }

            return new CachedDottedRuleSetTransition(
                postDotSymbol,
                stateFrame.DottedRuleSet,
                topCacheItem == null ? stateFrame.Origin : origin);
        }

        private void EarleyReductionOperation(int iLoc, DeterministicState fromEim, ISymbol transSym)
        {
            var fromAH = fromEim.DottedRuleSet;
            var originLoc = fromEim.Origin;

            var toAH = Goto(fromAH, transSym);
            if (toAH == null)
                return;

            AddEimPair(iLoc, toAH, originLoc);
        }

        private void LeoReductionOperation(int iLoc, CachedDottedRuleSetTransition fromLim)
        {
            var fromAH = fromLim.DottedRuleSet;
            var transSym = fromLim.Symbol;
            var originLoc = fromLim.Origin;

            var toAH = Goto(fromAH, transSym);
            if (toAH == null)
                return;

            AddEimPair(iLoc, toAH, originLoc);
        }

        private void AddEimPair(int iLoc, DottedRuleSet confirmedAH, int origLoc)
        {
            var confirmedEIM = new DeterministicState(confirmedAH, origLoc);
            var predictedAH = Goto(confirmedAH);
            Chart.Enqueue(iLoc, confirmedEIM);
            if (predictedAH == null)
                return;
            var predictedEIM = new DeterministicState(predictedAH, iLoc);
            Chart.Enqueue(iLoc, predictedEIM);
        }
                
        private static DottedRuleSet Goto(DottedRuleSet fromAH)
        {
            return fromAH.NullTransition;
        }

        private static DottedRuleSet Goto(DottedRuleSet fromAH, ISymbol symbol)
        {
            return fromAH.Reductions.GetOrReturnNull(symbol);         
        }

        private static DottedRuleSet Goto(DottedRuleSet fromAH, IToken token)
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
