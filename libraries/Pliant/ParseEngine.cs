using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pliant
{
    public class ParseEngine : IParseEngine
    {
        public IGrammar Grammar { get; private set; }

        public int Location { get; private set; }

        private Chart _chart;
        private NodeSet _nodeSet;

        public ParseEngine(IGrammar grammar)
        {
            _nodeSet = new NodeSet();
            Grammar = grammar;
            Initialize();
        }

        public IEnumerable<ILexerRule> GetExpectedLexerRules()
        {
            var earleySets = _chart.EarleySets;
            var currentIndex = earleySets.Count - 1;
            var currentEarleySet = earleySets[currentIndex];
            var scanStates = currentEarleySet.Scans;

            return from scanState in scanStates
                   let lexerRule = scanState.DottedRule.PostDotSymbol.Value as ILexerRule
                   group lexerRule by lexerRule.TokenType into rulesByTokenType
                   select rulesByTokenType.First();

        }

        public INode GetParseForest()
        {
            if (!IsAccepted())
                throw new Exception("Unable to parse expression.");
            
            var lastSet = _chart.EarleySets[_chart.Count - 1];
            var start = Grammar.Start;
            var completed = lastSet.Completions.First(x => x.Production.LeftHandSide.Equals(start));

            return completed.ParseNode;
        }

        public bool IsAccepted()
        {
            var lastEarleySet = _chart.EarleySets[_chart.Count - 1];
            var startStateSymbol = Grammar.Start;
            return lastEarleySet
                .Completions
                .Any(x =>
                    x.Origin == 0
                    && x.Production.LeftHandSide.Value == startStateSymbol.Value);

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
            IEarleySet earleySet = _chart.EarleySets[location];
            var tokenNode = new TokenNode(token, location, location + 1);
            for (int s = 0; s < earleySet.Scans.Count; s++)
            {
                var scanState = earleySet.Scans[s];
                Scan(scanState, location, tokenNode);
            }
        }

        private void Scan(IState scan, int j, ITokenNode tokenNode)
        {
            int i = scan.Origin;
            var currentSymbol = scan.DottedRule.PostDotSymbol.Value;
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
            IEarleySet earleySet = _chart.EarleySets[location];
            var resume = true;

            int p = 0,
                c = 0;

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
                    var prediction = earleySet.Predictions[p];
                    Predict(prediction, location);
                    p++;
                }
                else
                    resume = false;
            }
        }

        private void Predict(IState prediction, int j)
        {
            var nonTerminal = prediction.DottedRule.PostDotSymbol.Value as INonTerminal;
            foreach (var production in Grammar.RulesFor(nonTerminal))
            {
                PredictProduction(prediction, j, production);
            }
        }

        private void PredictProduction(IState evidence, int j, IProduction production)
        {
            // TODO: Pre-Compute Leo Items. If item is 1 step from being complete, add a transition item
            var predictedState = new State(production, 0, j);
            if (_chart.Enqueue(j, predictedState))
                Log("Predict", j, predictedState);

            var stateIsNullable = predictedState.Production.IsEmpty;
            if (stateIsNullable)
            {
                var aycockHorspoolState = evidence.NextState(j);

                var predictedParseNode = CreateNullParseNode(
                    predictedState.Production.LeftHandSide, j);

                aycockHorspoolState.ParseNode
                    = CreateParseNode(
                        aycockHorspoolState,
                        evidence.ParseNode,
                        predictedParseNode,
                        j);

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
            var earleySet = _chart.EarleySets[transitionState.Origin];
            var rootTransitionState = earleySet.FindTransitionState(
                transitionState.DottedRule.PreDotSymbol.Value);

            if (rootTransitionState == null)
                rootTransitionState = transitionState;

            var virtualParseNode = new VirtualNode(k, rootTransitionState, completed);

            var topmostItem = new State(
                transitionState.Production,
                transitionState.DottedRule.Position,
                transitionState.Origin,
                virtualParseNode);

            if (_chart.Enqueue(k, topmostItem))
                Log("Complete", k, topmostItem);
        }

        private void EarleyComplete(IState completed, int k)
        {
            int j = completed.Origin;
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
            TransitionState previousTransitionState = null;
            OptimizeReductionPathRecursive(searchSymbol, k, ref t_rule, ref previousTransitionState);
        }

        private void OptimizeReductionPathRecursive(
            ISymbol searchSymbol,
            int k,
            ref IState t_rule,
            ref TransitionState previousTransitionState)
        {
            var earleySet = _chart.EarleySets[k];
            var transitionState = earleySet.FindTransitionState(searchSymbol);
            if (transitionState != null)
            {
                t_rule = transitionState;
                return;
            }
            var sourceState = earleySet.FindSourceState(searchSymbol);
            if (sourceState == null)
                return;

            var sourceStateNext = sourceState.NextState();
            if (!IsQuasiComplete(sourceStateNext))
                return;

            t_rule = sourceStateNext;
            OptimizeReductionPathRecursive(
                sourceState.Production.LeftHandSide,
                sourceState.Origin,
                ref t_rule,
                ref previousTransitionState);

            if (t_rule == null)
                return;

            var currentTransitionState = new TransitionState(
                searchSymbol,
                t_rule,
                sourceState);

            if (previousTransitionState != null)
                previousTransitionState.NextTransition = currentTransitionState;

            if (_chart.Enqueue(k, currentTransitionState))
                Log("Transition", k, currentTransitionState);

            previousTransitionState = currentTransitionState;
        }

        private bool IsQuasiComplete(IState state)
        {
            // leo has a definition for quasi complete
            // where the item is either complete or "quasi complete"
            // "quasi complete" implies that the item has a NonTerminal after the 
            // post dot rule that is nullable, thereby making the state
            // complete by association. 
            // currently we only check for completeness, but a test case should
            // be developed to check for quasi completeness
            return state.DottedRule.IsComplete;
        }

        private INode CreateNullParseNode(ISymbol symbol, int location)
        {
            var symbolNode = _nodeSet.AddOrGetExistingSymbolNode(symbol, location, location);
            var nullNode = new TerminalNode('\0', location, location);
            symbolNode.AddUniqueFamily(nullNode);
            return symbolNode;
        }

        private INode CreateParseNode(
            IState nextState,
            INode w,
            INode v,
            int location)
        {
            Assert.IsNotNull(v, "v");
            var anyPreDotRuleNull = true;
            if (nextState.DottedRule.Position > 1)
            {
                var predotPrecursorSymbol = nextState
                    .Production
                    .RightHandSide[nextState.DottedRule.Position - 2];
                anyPreDotRuleNull = IsSymbolNullable(predotPrecursorSymbol);
            }
            var anyPostDotRuleNull = IsSymbolNullable(nextState.DottedRule.PostDotSymbol);
            if (anyPreDotRuleNull && !anyPostDotRuleNull)
                return v;

            IInternalNode internalNode;
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

        private bool IsSymbolNullable(INullable<ISymbol> symbol)
        {
            if (!symbol.HasValue)
                return true;

            return IsSymbolNullable(symbol.Value);
        }

        private bool IsSymbolNullable(ISymbol symbol)
        {
            if (symbol.SymbolType != SymbolType.NonTerminal)
                return false;
            var rules = Grammar.RulesFor(symbol as INonTerminal);
            return rules.Any(x => x.IsEmpty);
        }

        public void Reset()
        {
            Initialize();
        }

        private void Log(string operation, int origin, IState state)
        {
            Debug.Write(string.Format("{0}\t{1}", origin, state));
            Debug.WriteLine(string.Format("\t # {0}", operation));
        }
                
        private void LogScan(int origin, IState state, IToken token)
        {
            Debug.Write(string.Format("{0}\t{1}", origin, state));
            Debug.WriteLine(string.Format("\t # Scan {0}", token));
        }
    }
}
