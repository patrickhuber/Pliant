using Pliant.Forest;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Utilities;
using Pliant.Diagnostics;

namespace Pliant.Runtime
{
    public class ParseEngine : IParseEngine
    {
        public IGrammar Grammar { get; private set; }

        public int Location { get; private set; }

        public IReadOnlyChart Chart { get { return _chart; } }

        public ParseEngineOptions Options { get; private set; }

        public IStateFactory StateFactory { get; private set; }

        private const string PredictionLogName = "Predict";
        private const string StartLogName = "Start";
        private const string CompleteLogName = "Complete";
        private const string TransitionLogName = "Transition";

        private Chart _chart;
        private readonly ForestNodeSet _nodeSet;
        private readonly IDottedRuleRegistry _dottedRuleRegistry;

        public ParseEngine(IGrammar grammar)
            : this(grammar, new ParseEngineOptions(optimizeRightRecursion: true))
        {
        }

        public ParseEngine(IGrammar grammar, ParseEngineOptions options)
        {
            _dottedRuleRegistry = new GrammarSeededDottedRuleRegistry(grammar);
            StateFactory = new StateFactory(_dottedRuleRegistry);
            Options = options;
            _nodeSet = new ForestNodeSet();
            Grammar = grammar;
            Initialize();
        }
                
        private static readonly ILexerRule[] EmptyLexerRules = { };        
        private List<ILexerRule> _expectedLexerRules;

        /// <summary>
        /// Gets the expected list of lexer rules. The list returned will be mutated on subsequent calls for caching performance purposes.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<ILexerRule> GetExpectedLexerRules()
        {
            var earleySets = _chart.EarleySets;
            var currentIndex = earleySets.Count - 1;
            var currentEarleySet = earleySets[currentIndex];
            var scanStates = currentEarleySet.Scans;

            if (scanStates.Count == 0)
                return EmptyLexerRules;

            if (_expectedLexerRules is null)
                _expectedLexerRules = new List<ILexerRule>();
            else
                _expectedLexerRules.Clear();

            // loop over scan states and find lexer rules
            // add to reusable list
            for (int s = 0; s < scanStates.Count; s++)
            {
                var scanState = scanStates[s];
                var postDotSymbol = scanState.DottedRule.PostDotSymbol;
                if (postDotSymbol is null || postDotSymbol.SymbolType != SymbolType.LexerRule)
                    continue;

                var lexerRule = postDotSymbol as ILexerRule;
                _expectedLexerRules.Add(lexerRule);
            }

            return _expectedLexerRules;
        }

        public IInternalForestNode GetParseForestRootNode()
        {
            if (!IsAccepted())
                throw new Exception("Unable to parse expression.");

            var lastSet = _chart.EarleySets[_chart.Count - 1];
            var start = Grammar.Start;

            // PERF: Avoid Linq expressions due to lambda allocation
            for (int c = 0; c < lastSet.Completions.Count; c++)
            {
                var completion = lastSet.Completions[c];
                if (completion.DottedRule.Production.LeftHandSide.Equals(start) && completion.Origin == 0)
                {
                    return completion.ParseNode as IInternalForestNode;
                }
            }
            return null;
        }


        public bool IsAccepted()
        {
            var lastEarleySet = _chart.EarleySets[_chart.Count - 1];
            var startStateSymbol = Grammar.Start;

            // PERF: Avoid LINQ Any due to lambda allocation
            for (var c = 0; c < lastEarleySet.Completions.Count; c++)
            {
                var completion = lastEarleySet.Completions[c];
                if (completion.Origin == 0
                    && completion.DottedRule.Production.LeftHandSide.Value == startStateSymbol.Value)
                    return true;
            }
            return false;
        }

        private void Initialize()
        {
            Location = 0;
            _chart = new Chart();
            var startProductions = Grammar.StartProductions();
            for (var s = 0; s < startProductions.Count; s++)
            {
                var startProduction = startProductions[s];
                var startState = StateFactory.NewState(startProduction, 0, 0);
                if (_chart.Enqueue(0, startState))
                    Log(StartLogName, 0, startState);
            }
            ReductionPass(Location);
        }

        public bool Pulse(IToken token)
        {
            ScanPass(Location, token);

            var tokenRecognized = _chart.EarleySets.Count > Location + 1;
            if (!tokenRecognized)
                return false;

            Location++;
            ReductionPass(Location);

            _nodeSet.Clear();
            return true;
        }

        public bool Pulse(IReadOnlyList<IToken> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
                ScanPass(Location, tokens[i]);

            var tokenRecognized = _chart.EarleySets.Count > Location + 1;

            if (!tokenRecognized)
                return false;

            Location++;
            ReductionPass(Location);

            _nodeSet.Clear();
            return true;
        }

        public bool Errors(IReadOnlyList<IToken> tokens)
        {
            for (var e = 0; e < tokens.Count; e++)
                ErrorPass(Location, tokens[e]);

            var tokenRecognized = _chart.EarleySets.Count > Location + 1;

            if (!tokenRecognized)
                return false;

            Location++;
            ReductionPass(Location);

            _nodeSet.Clear();
            return true;
        }

        private void ErrorPass(int location, IToken token)
        {
            var earleySet = _chart.EarleySets[location];
            for (int s = 0; s < earleySet.Scans.Count; s++)
            {
                var scanState = earleySet.Scans[s];
                Error(scanState, location, token);
            }
        }

        private void Error(INormalState scan, int j, IToken token)
        {
            // to support missing tokens, add the expected token to the next state by scanning
            Scan(scan, j, token);

            if (_chart.Contains(j + 1, StateType.Normal, scan.DottedRule, scan.Origin))
                return;

            // to support error tokens, add the rule for the expected token to the next state
            var skipState = StateFactory.NewState(scan.DottedRule, scan.Origin);

            if (!_chart.Enqueue(j + 1, skipState))
                return;

            LogError(j + 1, skipState, token);
        }

        private void ScanPass(int location, IToken token)
        {
            var earleySet = _chart.EarleySets[location];
            for (int s = 0; s < earleySet.Scans.Count; s++)
            {
                var scanState = earleySet.Scans[s];
                Scan(scanState, location, token);
            }
        }

        private void Scan(INormalState scan, int j, IToken token)
        {
            var i = scan.Origin;
            var currentSymbol = scan.DottedRule.PostDotSymbol;
            var lexerRule = currentSymbol as ILexerRule;

            if (token.TokenType != lexerRule.TokenType)
                return;

            var dottedRule = _dottedRuleRegistry.GetNext(scan.DottedRule);
            if (_chart.Contains(j + 1, StateType.Normal, dottedRule, i))            
                return;
            
            var tokenNode = _nodeSet.AddOrGetExistingTokenNode(token);
            var parseNode = CreateParseNode(
                dottedRule,
                scan.Origin,
                scan.ParseNode,
                tokenNode,
                j + 1);
            var nextState = StateFactory.NewState(dottedRule, scan.Origin, parseNode);

            if (!_chart.Enqueue(j + 1, nextState))
                return;

            LogScan(j + 1, nextState, token);
        }

        private void ReductionPass(int location)
        {
            var earleySet = _chart.EarleySets[location];
            var resume = true;

            var p = 0;
            var c = 0;

            while (resume)
            {
                // is there a new completion?
                if (c < earleySet.Completions.Count)
                {
                    var completion = earleySet.Completions[c];
                    Complete(completion, location);
                    c++;
                }
                // is there a new prediction?
                else if (p < earleySet.Predictions.Count)
                {
                    var predictions = earleySet.Predictions;

                    var evidence = predictions[p];
                    Predict(evidence, location);

                    p++;
                }
                else
                    resume = false;
            }

            if(Options.OptimizeRightRecursion)
                Memoize(location);
        }

        private void Predict(INormalState evidence, int j)
        {
            var dottedRule = evidence.DottedRule;
            var nonTerminal = dottedRule.PostDotSymbol as INonTerminal;
            var rulesForNonTerminal = Grammar.RulesFor(nonTerminal);

            // PERF: Avoid boxing enumerable
            var rulesForNonTerminalCount = rulesForNonTerminal.Count;
            for (int p = 0; p < rulesForNonTerminalCount; p++)
            {
                var production = rulesForNonTerminal[p];
                PredictProduction(j, production);
            }

            var isNullable = Grammar.IsNullable(nonTerminal);
            if (isNullable)
                PredictAycockHorspool(evidence, j);
        }
                
        private void PredictProduction(int j, IProduction production)
        {
            var dottedRule = _dottedRuleRegistry.Get(production, 0);
            if (_chart.Contains(j, StateType.Normal, dottedRule, j))
                return;

            var predictedState = StateFactory.NewState(dottedRule, j);
            if (_chart.Enqueue(j, predictedState))
                Log(PredictionLogName, j, predictedState);
        }

        private void PredictAycockHorspool(INormalState evidence, int j)
        {
            var nullParseNode = CreateNullParseNode(evidence.DottedRule.PostDotSymbol, j);
            var dottedRule = _dottedRuleRegistry.GetNext(evidence.DottedRule);

            var evidenceParseNode = evidence.ParseNode as IInternalForestNode;
            IForestNode parseNode = null;

            if (evidenceParseNode is null)
            {
                parseNode = CreateParseNode(
                    dottedRule,
                    evidence.Origin,
                    null,
                    nullParseNode,
                    j);
            }
            else if (evidenceParseNode.Children.Count > 0
                && evidenceParseNode.Children[0].Children.Count > 0)
            {
                parseNode = CreateParseNode(
                    dottedRule,
                    evidence.Origin,
                    evidenceParseNode,
                    nullParseNode,
                    j);
            }

            if (_chart.Contains(j, StateType.Normal, dottedRule, evidence.Origin))
                return;

            var aycockHorspoolState = StateFactory.NewState(dottedRule, evidence.Origin, parseNode);
            if (_chart.Enqueue(j, aycockHorspoolState))
                Log(PredictionLogName, j, aycockHorspoolState);
        }

        private void Complete(INormalState completed, int k)
        {
            if (completed.ParseNode is null)
                completed.ParseNode = CreateNullParseNode(completed.DottedRule.Production.LeftHandSide, k);

            var earleySet = _chart.EarleySets[completed.Origin];
            var searchSymbol = completed.DottedRule.Production.LeftHandSide;

            var transitionState = earleySet.FindTransitionState(searchSymbol);
            if (transitionState != null)
            {
                LeoComplete(transitionState, completed, k);
            }
            else
            {
                EarleyComplete(completed, k);
            }
        }

        private void LeoComplete(ITransitionState transitionState, IState completed, int k)
        {
            // jump to the earley set of the transition
            var earleySet = _chart.EarleySets[transitionState.Origin];

            // look for another transition item here, this one will point to the correct reduction 
            ITransitionState topMost = earleySet.FindTransitionState(transitionState.Recognized);
            if (topMost == null)
                topMost = transitionState;
            
            var dottedRule = topMost.DottedRule;
            var origin = topMost.Reduction.Origin;

            var virtualParseNode = CreateVirtualParseNode(completed, k, topMost);

            var topmostItem = StateFactory.NewState(
                dottedRule,
                origin,
                virtualParseNode);

            if (_chart.Enqueue(k, topmostItem))
                Log(CompleteLogName, k, topmostItem);
        }

        private void EarleyComplete(INormalState completed, int k)
        {
            var j = completed.Origin;
            var sourceEarleySet = _chart.EarleySets[j];

            var predictionCount = sourceEarleySet.Predictions.Count;
            for (int p = 0; p < predictionCount; p++)
            {
                var prediction = sourceEarleySet.Predictions[p];
                if (!prediction.IsSource(completed.DottedRule.Production.LeftHandSide))
                    continue;

                var dottedRule = _dottedRuleRegistry.GetNext(prediction.DottedRule);
                var origin = prediction.Origin;

                // this will not create a node if the state already exists
                var parseNode = CreateParseNode(
                    dottedRule,
                    origin,
                    prediction.ParseNode,
                    completed.ParseNode,
                    k);

                if (_chart.Contains(k, StateType.Normal, dottedRule, origin))
                    continue;

                var nextState = StateFactory.NewState(dottedRule, origin, parseNode);

                if (_chart.Enqueue(k, nextState))
                    Log(CompleteLogName, k, nextState);
            }
        }

        private bool RuleIsRightRecursive(IDottedRule rule)
        {
            for (var s = rule.Production.RightHandSide.Count -1; s >= 0; s--)
            { 
                var symbol = rule.Production.RightHandSide[s];
                if (symbol.SymbolType != SymbolType.NonTerminal)
                    return false;
                if (symbol == rule.Production.LeftHandSide)
                    return true;
                if (Grammar.IsTransativeNullable(symbol as INonTerminal))
                    continue;

                // need to update grammar for IsRightRecursive(IDottedRule)
            }
            return false;
        }

        private void Memoize(int k)
        {
            var set = _chart.EarleySets[k];

            // loop through all completed items and memoize each        
            for (var c = 0; c < set.Completions.Count; c++)
            {
                var completion = set.Completions[c];
                var symbol = completion.DottedRule.Production.LeftHandSide;

                if (!RuleIsRightRecursive(completion.DottedRule))
                    continue;

                var prediction = set.FindSourceState(symbol);
                if (prediction == null)
                    continue;

                var next = _dottedRuleRegistry.GetNext(prediction.DottedRule);
                if (!IsQuasiComplete(next))
                    continue;

                var topMostCacheItem = FindTopMostTransition(prediction, symbol);
                var origin = topMostCacheItem is null
                            ? k
                            : topMostCacheItem.Origin;

                // this shouldn't be any completion, just the one that matches the leo constraints
                var transition = new TransitionState(symbol, completion, origin);

                // create a link between the previous transition item and the current transition item
                // this link is used during virtual parse node expansion
                var previous = FindTransition(prediction.Origin, symbol);
                if (previous != null)
                    previous.NextTransition = transition;

                if (_chart.Enqueue(k, transition))
                    Log(TransitionLogName, k, transition);
            }
        }

        /// <summary>
        /// Finds the transition for the given state origin and symbol
        /// </summary>
        /// <param name="state"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private ITransitionState FindTransition(int origin, ISymbol symbol)
        {   
            var set = _chart.EarleySets[origin];
            return set.FindTransitionState(symbol);
        }

        private ITransitionState FindTopMostTransition(IState state, ISymbol symbol)
        {
            ITransitionState topMostCacheItem = null;
            var origin = state.Origin;            
            while (true)
            {
                var next = FindTransition(origin, symbol);
                if (next == null)
                    break;
                topMostCacheItem = next;
                if (origin == next.Origin)
                    break;
                origin = topMostCacheItem.Origin;
            }
            return topMostCacheItem;
        }

        /// <summary>
        /// This should be pushed into the grammar analysis
        /// </summary>
        /// <param name="dottedRule"></param>
        /// <returns></returns>
        private bool IsQuasiComplete(IDottedRule dottedRule)
        {
            if (dottedRule.IsComplete)
                return true;

            // all postdot symbols are nullable
            for (var s = dottedRule.Position; s < dottedRule.Production.RightHandSide.Count; s++) 
            {
                var symbol = dottedRule.Production.RightHandSide[s];
                if (symbol.SymbolType != SymbolType.NonTerminal)
                    return false;

                var nonTerminal = symbol as INonTerminal;
                if (Grammar.RulesFor(nonTerminal).Count > 1)
                    return false;

                if (!Grammar.IsNullable(nonTerminal))
                    return false;

                // From Page 4 of Leo's paper:
                //
                // "on a non-empty deterministic reduction path there always
                //  exists a topmost item if S =+> S is impossible.
                //  The easiest way to avoid problems in this respect is to augment
                //  the grammar with a new start symbol S'.
                //  this means adding the rule S'=>S as the start."
                //
                // to fix this, check if S can derive S. Basically if we are in the Start state
                // and the Start state is found and is nullable, exit with false
                if (dottedRule.Production.LeftHandSide == Grammar.Start
                    && dottedRule.PostDotSymbol == Grammar.Start)
                    return false;

            }
            return true;
        }

        private static readonly TokenType EmptyTokenType = new TokenType(string.Empty);

        private IForestNode CreateNullParseNode(ISymbol symbol, int location)
        {
            var symbolNode = _nodeSet.AddOrGetExistingSymbolNode(symbol, location, location);
            var token = new Token(string.Empty, location, EmptyTokenType);
            var nullNode = new TokenForestNode(token, location, location);
            symbolNode.AddUniqueFamily(nullNode);
            return symbolNode;
        }

        private IForestNode CreateParseNode(
            IDottedRule nextDottedRule,
            int origin,
            IForestNode w,
            IForestNode v,
            int location)
        {
            Assert.IsNotNull(v, nameof(v));
            var anyPreDotRuleNull = true;
            if (nextDottedRule.Position > 1)
            {
                var predotPrecursorSymbol = nextDottedRule
                    .Production
                    .RightHandSide[nextDottedRule.Position - 2];
                anyPreDotRuleNull = IsSymbolTransativeNullable(predotPrecursorSymbol);
            }
            var anyPostDotRuleNull = IsSymbolTransativeNullable(nextDottedRule.PostDotSymbol);
            if (anyPreDotRuleNull && !anyPostDotRuleNull)
                return v;

            IInternalForestNode internalNode;
            if (anyPostDotRuleNull)
            {
                internalNode = _nodeSet
                    .AddOrGetExistingSymbolNode(
                        nextDottedRule.Production.LeftHandSide,
                        origin,
                        location);
            }
            else
            {
                internalNode = _nodeSet
                    .AddOrGetExistingIntermediateNode(
                        nextDottedRule,
                        origin,
                        location
                    );
            }

            // if w = null and y doesn't have a family of children (v)
            if (w is null)
                internalNode.AddUniqueFamily(v);

            // if w != null and y doesn't have a family of children (w, v)            
            else
                internalNode.AddUniqueFamily(w, v);

            return internalNode;
        }

        private VirtualForestNode CreateVirtualParseNode(IState completed, int k, ITransitionState rootTransitionState)
        {            
            if (_nodeSet.TryGetExistingVirtualNode(
                k,
                rootTransitionState,
                out VirtualForestNode virtualParseNode))
            {
                virtualParseNode.AddUniquePath(
                    new VirtualForestNodePath(rootTransitionState, completed.ParseNode));                
            }
            else
            {
                virtualParseNode = new VirtualForestNode(k, rootTransitionState, completed.ParseNode);
                _nodeSet.AddNewVirtualNode(virtualParseNode);
            }

            return virtualParseNode;
        }

        private bool IsSymbolTransativeNullable(ISymbol symbol)
        {
            if (symbol is null)
                return true;
            if (symbol.SymbolType != SymbolType.NonTerminal)
                return false;
            var nonTerminal = symbol as INonTerminal;
            return Grammar.IsNullable(nonTerminal);
        }

        public void Reset()
        {
            Initialize();
        }

        private void Log(string operation, int origin, IState state)
        {
            if (Options.LoggingEnabled)
                Debug.WriteLine(GetOriginStateOperationString(operation, origin, state));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in the critical path.")]
        private static string GetOriginStateOperationString(string operation, int origin, IState state)
        {
            return $"{origin,-9}{state,-100}{operation}";
        }

        private void LogScan(int origin, IState state, IToken token)
        {
            if (Options.LoggingEnabled)
                Debug.WriteLine($"{GetOriginStateOperationString("Scan", origin, state)} \"{token.Capture}\"");
        }

        private void LogError(int origin, IState state, IToken token)
        {
            if (Options.LoggingEnabled)
                Debug.WriteLine($"{GetOriginStateOperationString("Error", origin, state)} \"{token.Capture}\"");
        }
    }
}