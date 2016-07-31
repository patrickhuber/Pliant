using Pliant.Forest;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Utilities;
using Pliant.Diagnostics;
using Pliant.Collections;

namespace Pliant.Runtime
{
    public class ParseEngine : IParseEngine
    {
        public IGrammar Grammar { get; private set; }

        public int Location { get; private set; }

        public IReadOnlyChart Chart { get { return _chart; } }

        public ParseEngineOptions Options { get; private set; }

        private Chart _chart;
        private readonly ForestNodeSet _nodeSet;
        
        public ParseEngine(IGrammar grammar)
            : this(grammar, new ParseEngineOptions(optimizeRightRecursion: true))
        {
        }

        public ParseEngine(IGrammar grammar, ParseEngineOptions options)
        {
            Options = options;
            _nodeSet = new ForestNodeSet();
            Grammar = grammar;
            Initialize();
        }

        
        public List<ILexerRule> GetExpectedLexerRules()
        {
            var earleySets = _chart.EarleySets;
            var currentIndex = earleySets.Count - 1;
            var currentEarleySet = earleySets[currentIndex];
            var scanStates = currentEarleySet.Scans;

            var returnList = SharedPools.Default<List<ILexerRule>>().AllocateAndClear();
            var expectedRuleDictionary = SharedPools.Default<Dictionary<TokenType, ILexerRule>>().AllocateAndClear();

            // PERF: Avoid Linq Select, Where due to lambda allocation
            // PERF: Avoid foreach enumeration due to IEnumerable boxing
            for (int s = 0; s < scanStates.Count; s++)
            {
                var scanState = scanStates[s];
                var postDotSymbol = scanState.PostDotSymbol;
                if (postDotSymbol != null
                    && postDotSymbol.SymbolType == SymbolType.LexerRule)
                {
                    var lexerRule = postDotSymbol as ILexerRule;
                    if (!expectedRuleDictionary.ContainsKey(lexerRule.TokenType))
                    {
                        expectedRuleDictionary.Add(lexerRule.TokenType, lexerRule);
                        returnList.Add(lexerRule);
                    }
                }
            }
            SharedPools
                .Default<Dictionary<TokenType, ILexerRule>>()
                .Free(expectedRuleDictionary);
            return returnList;
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
                if (completion.Production.LeftHandSide.Equals(start) && completion.Origin == 0)
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
                    && completion.Production.LeftHandSide.Value == startStateSymbol.Value)
                    return true;
            }
            return false;
        }

        private void Initialize()
        {
            Location = 0;
            _chart = new Chart();
            foreach (var startProduction in Grammar.StartProductions())
            {
                var startState = new State(startProduction, 0, 0);
                if (_chart.Enqueue(0, startState))
                    Log("Start", 0, startState);
            }
            ReductionPass(Location);
        }

        public bool Pulse(IToken token)
        {
            _nodeSet.Clear();
            ScanPass(Location, token);

            var tokenRecognized = _chart.EarleySets.Count > Location + 1;
            if (!tokenRecognized)
                return false;

            Location++;
            ReductionPass(Location);

            return true;
        }

        private void ScanPass(int location, IToken token)
        {
            var earleySet = _chart.EarleySets[location];
            var tokenNode = new TokenForestNode(token, location, location + 1);
            for (int s = 0; s < earleySet.Scans.Count; s++)
            {
                var scanState = earleySet.Scans[s];
                Scan(scanState, location, tokenNode);
            }
        }

        private void Scan(IState scan, int j, ITokenForestNode tokenNode)
        {
            var i = scan.Origin;
            var currentSymbol = scan.PostDotSymbol;
            var lexerRule = currentSymbol as ILexerRule;

            var token = tokenNode.Token;
            if (token.TokenType == lexerRule.TokenType)
            {
                var nextState = scan.NextState();
                var parseNode = CreateParseNode(
                    nextState,
                    scan.ParseNode,
                    tokenNode,
                    j + 1);
                nextState.ParseNode = parseNode;

                if (_chart.Enqueue(j + 1, nextState))
                    LogScan(j + 1, nextState, token);
            }
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
                    var evidence = earleySet.Predictions[p];
                    Predict(evidence, location);
                    p++;
                }
                else
                    resume = false;
            }
        }

        private void Predict(IState evidence, int j)
        {
            var nonTerminal = evidence.PostDotSymbol as INonTerminal;
            var rulesForNonTerminal = Grammar.RulesFor(nonTerminal);
            // PERF: Avoid boxing enumerable

            for (int p =0; p<rulesForNonTerminal.Count;p++)

            {
                var production = rulesForNonTerminal[p];
                PredictProduction(evidence, j, production);
            }
        }

        private void PredictProduction(IState evidence, int j, IProduction production)
        {
            // TODO: Pre-Compute Leo Items. If item is 1 step from being complete, add a transition item
            var predictedState = new State(production, 0, j);
            if (_chart.Enqueue(j, predictedState))
                Log("Predict", j, predictedState);

            var isNullable = Grammar.IsNullable(evidence.PostDotSymbol as INonTerminal);
            if (isNullable)
            {
                var nullParseNode = CreateNullParseNode(evidence.PostDotSymbol, j);
                var aycockHorspoolState = evidence.NextState();
                var evidenceParseNode = evidence.ParseNode as IInternalForestNode;
                if (evidenceParseNode == null)
                    aycockHorspoolState.ParseNode = CreateParseNode(aycockHorspoolState, null, nullParseNode, j);
                else if (evidenceParseNode.Children.Count > 0 
                    && evidenceParseNode.Children[0].Children.Count > 0)
                {
                    var firstChildNode = evidenceParseNode;
                    var parseNode = CreateParseNode(aycockHorspoolState, firstChildNode, nullParseNode, j);
                    aycockHorspoolState.ParseNode = parseNode;
                }
                if (_chart.Enqueue(j, aycockHorspoolState))
                    Log("Predict", j, aycockHorspoolState);
            }
        }

        private void Complete(IState completed, int k)
        {
            if (completed.ParseNode == null)
                completed.ParseNode = CreateNullParseNode(completed.Production.LeftHandSide, k);
            
            var earleySet = _chart.EarleySets[completed.Origin];
            var searchSymbol = completed.Production.LeftHandSide;

            if(Options.OptimizeRightRecursion)
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
                transitionState.PreDotSymbol);

            if (rootTransitionState == null)
                rootTransitionState = transitionState;

            var virtualParseNode = new NewVirtualForestNode(k, rootTransitionState, completed.ParseNode);

            var topmostItem = new State(
                transitionState.Production,
                transitionState.Position,
                transitionState.Origin,
                virtualParseNode);

            if (_chart.Enqueue(k, topmostItem))
                Log("Complete", k, topmostItem);
        }

        private void EarleyComplete(IState completed, int k)
        {
            var j = completed.Origin;
            var sourceEarleySet = _chart.EarleySets[j];
                        
            for (int p = 0; p < sourceEarleySet.Predictions.Count; p++)
            {
                var prediction = sourceEarleySet.Predictions[p];
                if (!prediction.IsSource(completed.Production.LeftHandSide))
                    continue;

                var i = prediction.Origin;
                var nextState = prediction.NextState();

                var parseNode = CreateParseNode(
                    nextState,
                    prediction.ParseNode,
                    completed.ParseNode,
                    k);
                nextState.ParseNode = parseNode;

                if (_chart.Enqueue(k, nextState))
                    Log("Complete", k, nextState);
            }
        }

        private void OptimizeReductionPath(ISymbol searchSymbol, int k)
        {
            IState t_rule = null;
            ITransitionState previousTransitionState = null;
            OptimizeReductionPathRecursive(searchSymbol, k, ref t_rule, ref previousTransitionState);
        }

        private void OptimizeReductionPathRecursive(
            ISymbol searchSymbol,
            int k,
            ref IState t_rule,
            ref ITransitionState previousTransitionState)
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
            if (sourceState == null)
                return;

            // and [B-> aA.b, k] is quasi complete (is b null)
            if (!IsNextStateQuasiComplete(sourceState))
                return;

            // then t_rule := [B->aAb.]; t_pos=k;
            t_rule = sourceState.NextState();

            // T_Update(I0...Ik, B);
            OptimizeReductionPathRecursive(
                sourceState.Production.LeftHandSide,
                sourceState.Origin,
                ref t_rule,
                ref previousTransitionState);

            if (t_rule == null)
                return;

            ITransitionState currentTransitionState = null;
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
                Log("Transition", k, currentTransitionState);

            previousTransitionState = currentTransitionState;
        }

        /// <summary>
        /// Implements a check for leo quasi complete items
        /// </summary>
        /// <param name="state">the state to check for quasi completeness</param>
        /// <returns>true if quasi complete, false otherwise</returns>
        private bool IsNextStateQuasiComplete(IState state)
        {
            var ruleCount = state.Production.RightHandSide.Count;
            if (ruleCount == 0)
                return true;

            var nextStatePosition = state.Position + 1;
            var isComplete = nextStatePosition == state.Production.RightHandSide.Count;
            if (isComplete)
                return true;

            // if all subsequent symbols are nullable
            for (int i = nextStatePosition; i < state.Production.RightHandSide.Count; i++)
            {
                var nextSymbol = state.Production.RightHandSide[nextStatePosition];
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
                if (state.Production.LeftHandSide == Grammar.Start &&
                    nextSymbol == Grammar.Start)
                    return false;
            }

            return true;
        }

        private IForestNode CreateNullParseNode(ISymbol symbol, int location)
        {
            var symbolNode = _nodeSet.AddOrGetExistingSymbolNode(symbol, location, location);
            var token = new Token(string.Empty, location, new TokenType(string.Empty));
            var nullNode = new TokenForestNode(token, location, location);
            symbolNode.AddUniqueFamily(nullNode);
            return symbolNode;
        }        

        private IForestNode CreateParseNode(
            IState nextState,
            IForestNode w,
            IForestNode v,
            int location)
        {
            Assert.IsNotNull(v, nameof(v));
            var anyPreDotRuleNull = true;
            if (nextState.Position > 1)
            {
                var predotPrecursorSymbol = nextState
                    .Production
                    .RightHandSide[nextState.Position - 2];
                anyPreDotRuleNull = IsSymbolNullable(predotPrecursorSymbol);
            }
            var anyPostDotRuleNull = IsSymbolNullable(nextState.PostDotSymbol);
            if (anyPreDotRuleNull && !anyPostDotRuleNull)
                return v;

            IInternalForestNode internalNode = null; 
            if (anyPostDotRuleNull)
            {
                internalNode = _nodeSet
                    .AddOrGetExistingSymbolNode(
                        nextState.Production.LeftHandSide,
                        nextState.Origin,
                        location);
            }
            else
            {
                internalNode = _nodeSet
                    .AddOrGetExistingIntermediateNode(
                        nextState,
                        nextState.Origin,
                        location
                    );
            }
                        
            // if w = null and y doesn't have a family of children (v)
            if (w == null)
                internalNode.AddUniqueFamily(v);

            // if w != null and y doesn't have a family of children (w, v)            
            else
                internalNode.AddUniqueFamily(w, v);

            return internalNode;
        }

        private bool IsSymbolNullable(ISymbol symbol)
        {
            if (symbol == null)
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

        private static void Log(string operation, int origin, IState state)
        {
            LogOriginStateOperation(operation, origin, state);
            Debug.WriteLine(string.Empty);
        }

        private static void LogOriginStateOperation(string operation, int origin, IState state)
        {
            Debug.Write($"{origin.ToString().PadRight(50)}{state.ToString().PadRight(50)}{operation}");
        }

        private static void LogScan(int origin, IState state, IToken token)
        {
            LogOriginStateOperation("Scan", origin, state);
            Debug.WriteLine($" {token.Value}");
        }        
    }
}