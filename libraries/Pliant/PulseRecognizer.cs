using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class PulseRecognizer
    {
        /// <summary>
        /// The grammar used in the parse
        /// </summary>
        public IGrammar Grammar { get; private set; }

        /// <summary>
        /// The state of the parse
        /// </summary>
        public Chart Chart { get; private set; }

        /// <summary>
        /// The current location within the parse
        /// </summary>
        public int Location { get; private set; }

        private NodeSet _nodeSet;

        public PulseRecognizer(IGrammar grammar)
        {
            _nodeSet = new NodeSet();
            
            Grammar = grammar;
            Initialization();
        }
        
        private void Initialization()
        {
            Location = 0;
            Chart = new Chart();
            foreach (var startProduction in Grammar.StartProductions())
            {
                var startState = new State(startProduction, 0, 0);
                if (Chart.Enqueue(0, startState))
                    Log("Start", 0, startState);
            }
            ReductionPass(Location);
        }

        public bool Pulse(char token)
        {
            ScanPass(Location, token);

            bool tokenNotRecognized = Chart.EarleySets.Count <= Location + 1;
            if (tokenNotRecognized)
                return false;  

            // Move to next earleySet
            Location++;          
            
            ReductionPass(Location);

            _nodeSet.Clear();
            return true;
        }
        
        private void ScanPass(int location, char token)
        {
            IEarleySet earleySet = Chart.EarleySets[location];
            var scanNode = new TerminalNode(token, location, location + 1);
            for (int s = 0; s < earleySet.Scans.Count; s++)
            {
                var scanState = earleySet.Scans[s];
                Scan(scanState, location, scanNode);
            }
        }

        private void Scan(IState scan, int j, ITerminalNode scanNode)
        {
            int i = scan.Origin;
            var currentSymbol = scan.DottedRule.PostDotSymbol.Value;
            var terminal = currentSymbol as ITerminal;

            var token = scanNode.Capture;
            if (terminal.IsMatch(token))
            {
                var scanState = scan.NextState();
                var parseNode = CreateParseNode(
                    scan, 
                    scanState, 
                    scan.ParseNode, 
                    scanNode, 
                    j + 1);
                scanState.ParseNode = parseNode;

                if (Chart.Enqueue(j + 1, scanState))
                    LogScan(j + 1, scanState, token);
            }
        }
        
        private void ReductionPass(int location)
        {
            IEarleySet earleySet = Chart.EarleySets[location];
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

        private void PredictProduction(IState prediction, int j, IProduction production)
        {
            var predictedState = new State(production, 0, j);
            if (Chart.Enqueue(j, predictedState))
                Log("Predict", j, predictedState);

            var stateIsNullable = predictedState.Production.IsEmpty;
            if (stateIsNullable)
            {
                var aycockHorspoolState = prediction.NextState(j);
                var nullParseNode = _nodeSet.AddOrGetExistingSymbolNode(
                    predictedState.Production.LeftHandSide, 
                    j, j);
                var parseNode = CreateParseNode(
                    aycockHorspoolState,
                    prediction,
                    prediction.ParseNode,
                    nullParseNode,
                    j);
                aycockHorspoolState.ParseNode = parseNode;
                if(Chart.Enqueue(j, aycockHorspoolState))
                    Log("Predict", j, aycockHorspoolState);
            }
        }

        private void Complete(IState completed, int k)
        {
            if (completed.ParseNode == null)
            {
                // this is a null production
                completed.ParseNode = _nodeSet.AddOrGetExistingSymbolNode(
                    completed.Production.LeftHandSide, k, k);
            }

            var earleySet = Chart.EarleySets[completed.Origin];
            var searchSymbol = completed.Production.LeftHandSide;
            
            OptimizeReductionPath(searchSymbol, completed.Origin, Chart);
            
            var transitiveState = FindTransitiveState(earleySet, searchSymbol);
            if (transitiveState != null)
            {
                LeoComplete(completed, k, transitiveState);
            }
            else
            {
                EarleyComplete(completed, k);
            }
        }

        private void LeoComplete(IState completed, int k, IState transitiveState)
        {
            // TODO: Find the Predicted state from k-1 that produced the item
            var parseNode = CreateParseNode(
                transitiveState,
                null,
                transitiveState.ParseNode,
                null,
                k);
            var topmostItem = new State(
                transitiveState.Production,
                transitiveState.DottedRule.Position,
                transitiveState.Origin,
                parseNode);
            if (Chart.Enqueue(k, topmostItem))
                Log("Complete", k, topmostItem);
        }

        private void EarleyComplete(IState completed, int k)
        {
            int j = completed.Origin;
            var sourceEarleySet = Chart.EarleySets[j];
            for (int p = 0; p < sourceEarleySet.Predictions.Count; p++)
            {
                var prediction = sourceEarleySet.Predictions[p];
                if (!IsSourceState(completed.Production.LeftHandSide, prediction))
                    continue;

                var i = prediction.Origin;
                var nextState = prediction.NextState();
                var parseNode = CreateParseNode(
                    completed,
                    nextState,
                    completed.ParseNode,
                    nextState.ParseNode,
                    k);
                nextState.ParseNode = parseNode;
 
                if (Chart.Enqueue(k, nextState))
                    Log("Complete", k, nextState);                
            }
        }
        
        private void OptimizeReductionPath(ISymbol searchSymbol, int k, Chart chart)
        {
            IState t_rule = null;
            OptimizeReductionPathRecursive(searchSymbol, k, chart, ref t_rule);
        }

        private void OptimizeReductionPathRecursive(ISymbol searchSymbol, int k, Chart chart, ref IState t_rule)
        {
            var earleySet = chart.EarleySets[k];
            var transitiveState = FindTransitiveState(earleySet, searchSymbol);
            if (transitiveState != null)
            {
                t_rule = transitiveState;
                return;
            }

            var sourceState = FindSourceState(earleySet, searchSymbol);
            if (sourceState == null)
                return;

            var sourceStateNext = sourceState.NextState();
            if (!IsQuasiComplete(sourceStateNext))
                return;

            t_rule = sourceStateNext;
            OptimizeReductionPathRecursive(
                sourceState.Production.LeftHandSide, 
                sourceState.Origin, 
                chart, 
                ref t_rule);
            
            if (t_rule == null)
                return;
            
            var transitionItem = new TransitionState(
                searchSymbol,
                t_rule);
            
            if (chart.Enqueue(k, transitionItem))
                Log("Transition", k, transitionItem);            
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

        private IState FindSourceState(IEarleySet earleySet, ISymbol searchSymbol)
        {
            // TODO: speed up by using a index lookup
            var sourceItemCount = 0;
            IState sourceItem = null;
            for (int s = 0; s < earleySet.Predictions.Count; s++)
            {
                var state = earleySet.Predictions[s];
                if (IsSourceState(searchSymbol, state))
                {
                    bool moreThanOneSourceItemExists = sourceItemCount > 0;
                    if (moreThanOneSourceItemExists)
                        return null;
                    sourceItemCount++;
                    sourceItem = state;
                }
            }
            return sourceItem;
        }

        private bool IsSourceState(ISymbol searchSymbol, IState state)
        {
            if (state.DottedRule.IsComplete)
                return false;
            return state.DottedRule.PostDotSymbol.Value.Equals(searchSymbol);
        }

        private IState FindTransitiveState(IEarleySet earleySet, ISymbol searchSymbol)
        {
            for (int t = 0; t < earleySet.Transitions.Count; t++)
            {
                var transitionState = earleySet.Transitions[t] as TransitionState;
                if (transitionState.Recognized.Equals(searchSymbol))
                    return transitionState;
            }
            return null;
        }
        
        public void Reset()
        {
            Initialization();
        }

        public bool IsAccepted()
        {
            var lastEarleySet = Chart.EarleySets[Chart.Count - 1];
            var startStateSymbol = Grammar.Start;
            return lastEarleySet
                .Completions
                .Any(x =>
                    x.Origin == 0
                    && x.Production.LeftHandSide.Value == startStateSymbol.Value);
        }

        private INode CreateParseNode(
            IState originalState, 
            IState nextState,
            INode w,
            INode v,
            int location)
        {
            // originalState is pre transition
            // nextState is post transition
            var anyPostDotRuleNull = IsSymbolNullable(nextState.DottedRule.PostDotSymbol);
            var anyPreDotRuleNull = IsSymbolNullable(originalState.DottedRule.PreDotSymbol);
            if (anyPostDotRuleNull && !anyPostDotRuleNull)
                return v;

            IInternalNode internalNode;
            if (anyPostDotRuleNull)
            {
                internalNode = _nodeSet
                    .AddOrGetExistingSymbolNode(
                        originalState.Production.LeftHandSide,
                        originalState.Origin,
                        location);
            }
            else // should this be the trigger state or the source state ??
            {
                internalNode = _nodeSet
                    .AddOrGetExistingIntermediateNode(
                        originalState,
                        originalState.Origin,
                        location
                    );
            }
            
            if(v == null)
                v = new TerminalNode('\0', nextState.Origin, location);

            // if w = null and y doesn't have a family of children (v)
            if (w == null)
                internalNode.AddUniqueFamily(v);            

            // if w != null and y doesn't have a family of children (w, v)
            else
                internalNode.AddUniqueFamily(w, v);            

            return internalNode;            
        }

        private INode CreateParseNode(IState state, int origin, int location, INode w, INode v)
        {
            var symbolNode = _nodeSet
                .AddOrGetExistingSymbolNode(
                    state.Production.LeftHandSide,
                    origin,
                    location);

            if (v == null)
                v = new TerminalNode('\0', location, location);

            if (w == null)
                symbolNode.AddUniqueFamily(v);
            else
                symbolNode.AddUniqueFamily(v, w);

            return symbolNode;
        }

        private bool IsSymbolNullable(INullable<ISymbol> symbol)
        {
            if (!symbol.HasValue)
                return true;

            if (symbol.Value.SymbolType != SymbolType.NonTerminal)
                return false;

            var postDotRules = Grammar.RulesFor(symbol.Value as INonTerminal);
            return postDotRules.Any(x => x.IsEmpty);            
        }

        private void Log(string operation, int origin, IState state)
        {
            Debug.Write(string.Format("{0}\t{1}", origin, state));
            Debug.WriteLine(string.Format("\t # {0}", operation));
        }

        private void LogScan(int origin, IState state, char token)
        {
            Debug.Write(string.Format("{0}\t{1}", origin, state));
            Debug.WriteLine(string.Format("\t # Scan {0}", token));
        }
    }
}
