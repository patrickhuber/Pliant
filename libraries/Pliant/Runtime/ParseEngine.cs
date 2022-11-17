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
        private const string PredictionAycockHorspoolLogName = "PredictAH";
        private const string StartLogName = "Start";
        private const string CompleteLogName = "Complete";
        private const string LeoCompleteLogName = "LeoComplete";
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
            var acceptedCompletion = FindAcceptedCompletion(_chart.Count - 1);
            if (acceptedCompletion == null)
                new Exception("Unable to parse expression.");
            return acceptedCompletion.ParseNode as IInternalForestNode;
        }

        private INormalState FindAcceptedCompletion(int location)
        {
            var set = _chart.EarleySets[location];
            var start = Grammar.Start;
            var reductions = set.FindReductions(start);
            var reductionCount = reductions.Count;
            for (var c = 0; c < reductionCount; c++)
            {
                var completion = reductions[c];
                if (completion.Origin == 0
                    && completion.DottedRule.Production.LeftHandSide.Value.Equals(start.Value))
                    return completion;

            }
            return null;
        }

        public bool IsAccepted()
        {
            var acceptedCompletion = FindAcceptedCompletion(_chart.Count - 1);
            return !(acceptedCompletion is null);
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
            
            var tokenNode = _nodeSet.AddOrGetExistingTokenNode(token, j+ 1);
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

            var isNullable = Grammar.IsTransativeNullable(nonTerminal);
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
                Log(PredictionAycockHorspoolLogName, j, aycockHorspoolState);
        }

        private void Complete(INormalState completed, int k)
        {
            var set = _chart.EarleySets[completed.Origin];
            var searchSymbol = completed.DottedRule.Production.LeftHandSide;

            // Find a transition state in the current set
            var transitionState = set.FindTransitionState(searchSymbol);
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
            var dottedRule = transitionState.DottedRule;
            var origin = transitionState.Origin;

            if (_chart.Contains(k, StateType.Normal, dottedRule, origin))
                return;
            
            var parseNode = CreateDynamicParseNode(completed, k, transitionState);
            
            var topmostItem = StateFactory.NewState(
                dottedRule,
                origin,
                parseNode);

            if (_chart.Enqueue(k, topmostItem))
                Log(LeoCompleteLogName, k, topmostItem);
        }

        private void EarleyComplete(INormalState completed, int k)
        {
            // TODO: check if this is needed. Should only be a factor in aycock horspool completions
            if (completed.ParseNode is null)
                completed.ParseNode = CreateNullParseNode(completed.DottedRule.Production.LeftHandSide, k);

            var j = completed.Origin;
            var set = _chart.EarleySets[j];

            var sources = set.FindSourceStates(completed.DottedRule.Production.LeftHandSide);
            var predictionCount = sources.Count;

            for (var p = 0; p < predictionCount; p++)
            {
                var prediction = sources[p];

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

        private void Memoize(int location)
        {
            var set = _chart.EarleySets[location];
            var counts = new Dictionary<ISymbol, int>();
            var rules = new Dictionary<ISymbol, INormalState>();

            // for every postdot symbol in iES do
            for (var p = 0; p < set.Predictions.Count; p++)
            {
                // LeoEligible(rule.PostDotSymbol) = LeoUnique(rule.PostDotSybol) AND RightRecursive(rule)
                var rule = set.Predictions[p];

                if (counts.TryGetValue(rule.DottedRule.PostDotSymbol, out var count))
                {
                    counts[rule.DottedRule.PostDotSymbol] = count + 1;
                    continue;
                }

                rules[rule.DottedRule.PostDotSymbol] = rule;
                counts[rule.DottedRule.PostDotSymbol] = 1;
            }

            foreach (var postDotSymbol in counts.Keys)
            {
                var count = counts[postDotSymbol];

                // LeoUnique(rule.PostDotSybol)
                if (count != 1)
                    continue;

                var prediction = rules[postDotSymbol];
                if (!Grammar.IsRightRecursive(prediction.DottedRule.Production))
                    continue;

                var next = _dottedRuleRegistry.GetNext(prediction.DottedRule);
                if (!IsQuasiComplete(next))
                    continue;
                
                // top and bottom represent the top and bottom of the reduction path
                // for leo items, there should only be one or two
                // it is possible for top and bottom to point to the same state
                var top = FindReduction(location, prediction);
                if(top is null)
                    continue;
                var transition = CreateTopTransition(top, prediction, postDotSymbol);
                if (set.Enqueue(transition))
                    Log(TransitionLogName, location, transition);                              
            }
        }

        private ITransitionState CreateTopTransition(IState top, IState bottom, ISymbol symbol)
        {
            var origin = top.Origin;
            ITransitionState topTransition = null;
            while (true)
            {
                var set = Chart.EarleySets[origin];
                var nextTransition = set.FindTransitionState(symbol);

                if (nextTransition == null)
                    break;

                topTransition = nextTransition;
                if (origin == nextTransition.Origin)
                    break;

                origin = topTransition.Origin;
            }
            
            var transition = new TransitionState(symbol, top.DottedRule, bottom, topTransition == null ? top.Origin : origin);

            // link to previous transition to assist with parse forest generation            
            var bottomSet = Chart.EarleySets[bottom.Origin];
            var previousTransition = bottomSet.FindTransitionState(symbol);
            if (previousTransition is not null)
            {
                previousTransition.Next = transition;
                transition.First = previousTransition.First;
            }
            else
            {
                // link to self if no previous transition
                transition.First = transition;
            }

            // does prediction contain the parse node I need?
            return transition;
        }

        private INormalState FindReduction(int location, INormalState prediction)
        {
            var set = _chart.EarleySets[location];
            INormalState reduction = null;

            var reductions = set.FindReductions(prediction.DottedRule.PostDotSymbol);
            var reductionCount = reductions.Count;

            for (var c = 0; c < reductionCount; c++)
            {
                var completion = reductions[c];

                // from leo's paper:
                // if set contains one (B -> a*AB, k) and (B -> aA*B, k) is quasi complete
                // then add (B -> aAB*, k) 
                //if (completion.DottedRule.Production != prediction.DottedRule.Production)
                //    continue;

                if (reduction is null)
                {
                    reduction = completion;
                    continue;
                }

                // top will always be the stat with the smallest origin
                if (completion.Origin < reduction.Origin)
                    reduction = completion;
            }
            return reduction;
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
            var rightHandSideCount = dottedRule.Production.RightHandSide.Count;
            for (var s = dottedRule.Position; s < rightHandSideCount; s++) 
            {
                var symbol = dottedRule.Production.RightHandSide[s];
                if (symbol.SymbolType != SymbolType.NonTerminal)
                    return false;

                var nonTerminal = symbol as INonTerminal;
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
                    && symbol == Grammar.Start)
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

        private DynamicForestNode CreateDynamicParseNode(IState completed, int k, ITransitionState transition)
        {
            if (_nodeSet.TryGetExistingDynamicNode(k, transition.Recognized, transition.Origin, out var node))
                return node;
            var parseNode = new DynamicForestNode(
                new DynamicForestNodeLinkAdapter(transition.First),
                completed.ParseNode,
                Location);
            _nodeSet.AddNewDynamicNode(parseNode);
            return parseNode;
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