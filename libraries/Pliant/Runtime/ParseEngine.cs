﻿using Pliant.Forest;
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

            var isNullable = Grammar.IsTransativeNullable(nonTerminal);
            if (isNullable)
                PredictAycockHorspool(evidence, j);
        }
                
        private void PredictProduction(int j, IProduction production)
        {
            var dottedRule = _dottedRuleRegistry.Get(production, 0);
            if (_chart.Contains(j, StateType.Normal, dottedRule, j))
                return;

            // TODO: Pre-Compute Leo Items. If item is 1 step from being complete, add a transition item
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

            if (Options.OptimizeRightRecursion)
                OptimizeReductionPath(searchSymbol, completed.Origin);

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
            var earleySet = _chart.EarleySets[transitionState.Index];
            var rootTransitionState = earleySet.FindTransitionState(
                transitionState.DottedRule.PreDotSymbol);

            if (rootTransitionState is null)
                rootTransitionState = transitionState;

            var virtualParseNode = CreateVirtualParseNode(completed, k, rootTransitionState);

            var dottedRule = transitionState.DottedRule;
            var topmostItem = StateFactory.NewState(
                dottedRule,
                transitionState.Origin,
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

        private void OptimizeReductionPath(ISymbol searchSymbol, int k)
        {
            IState t_rule = null;
            ITransitionState previousTransitionState = null;

            var visited = SharedPools.Default<HashSet<IState>>().AllocateAndClear();
            OptimizeReductionPathRecursive(searchSymbol, k, ref t_rule, ref previousTransitionState, visited);
            SharedPools.Default<HashSet<IState>>().ClearAndFree(visited);
        }

        private void OptimizeReductionPathRecursive(
            ISymbol searchSymbol,
            int k,
            ref IState t_rule,
            ref ITransitionState previousTransitionState,
            HashSet<IState> visited)
        {
            var earleySet = _chart.EarleySets[k];

            // if Ii contains a transitive item of the for [B -> b., A, k]
            var transitionState = earleySet.FindTransitionState(searchSymbol);
            if (transitionState != null)
            {
                // then t_rule := B-> b.; t_pos = k;
                previousTransitionState = transitionState;
                t_rule = transitionState;
                return;
            }

            // else if Ii contains exactly one item of the form [B -> a.Ab, k]
            var sourceState = earleySet.FindSourceState(searchSymbol);
            if (sourceState is null)
                return;

            if (!visited.Add(sourceState))
                return;

            // and [B-> aA.b, k] is quasi complete (is b null)
            if (!IsNextStateQuasiComplete(sourceState))
                return;

            // then t_rule := [B->aAb.]; t_pos=k;
            t_rule = StateFactory.NextState(sourceState);

            if (sourceState.Origin != k)
                visited.Clear();

            // T_Update(I0...Ik, B);
            OptimizeReductionPathRecursive(
                sourceState.DottedRule.Production.LeftHandSide,
                sourceState.Origin,
                ref t_rule,
                ref previousTransitionState,
                visited);

            if (t_rule is null)
                return;

            ITransitionState currentTransitionState;
            if (previousTransitionState != null)
            {
                currentTransitionState = new TransitionState(
                  searchSymbol,
                  t_rule,
                  sourceState,
                  previousTransitionState.Index);

                previousTransitionState.NextTransition = currentTransitionState;
            }
            else
                currentTransitionState = new TransitionState(
                  searchSymbol,
                  t_rule,
                  sourceState,
                  k);

            if (_chart.Enqueue(k, currentTransitionState))
            {
                Log(TransitionLogName, k, currentTransitionState);
            }

            previousTransitionState = currentTransitionState;
        }

        /// <summary>
        /// Implements a check for leo quasi complete items
        /// </summary>
        /// <param name="state">the state to check for quasi completeness</param>
        /// <returns>true if quasi complete, false otherwise</returns>
        private bool IsNextStateQuasiComplete(IState state)
        {
            var ruleCount = state.DottedRule.Production.RightHandSide.Count;
            if (ruleCount == 0)
                return true;

            var nextStatePosition = state.DottedRule.Position + 1;
            var isComplete = nextStatePosition == state.DottedRule.Production.RightHandSide.Count;
            if (isComplete)
                return true;

            // if all subsequent symbols are nullable
            for (int i = nextStatePosition; i < state.DottedRule.Production.RightHandSide.Count; i++)
            {
                var nextSymbol = state.DottedRule.Production.RightHandSide[nextStatePosition];
                var isSymbolNullable = IsSymbolNullable(nextSymbol);
                if (!isSymbolNullable)
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
                if (state.DottedRule.Production.LeftHandSide == Grammar.Start &&
                    nextSymbol == Grammar.Start)
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

        private bool IsSymbolNullable(ISymbol symbol)
        {
            if (symbol is null)
                return true;
            if (symbol.SymbolType != SymbolType.NonTerminal)
                return false;
            var nonTerminal = symbol as INonTerminal;
            return Grammar.IsNullable(nonTerminal);
        }

        private bool IsSymbolTransativeNullable(ISymbol symbol)
        {
            if (symbol is null)
                return true;
            if (symbol.SymbolType != SymbolType.NonTerminal)
                return false;
            var nonTerminal = symbol as INonTerminal;
            return Grammar.IsTransativeNullable(nonTerminal);
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