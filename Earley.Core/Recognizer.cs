using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class Recognizer
    {
        private IGrammar _grammar;

        public Recognizer(IGrammar grammar)
        {
            Assert.IsNotNull(grammar, "grammar");
            _grammar = grammar;
        }

        public Chart Parse(IEnumerable<char> tokens)
        {
            var chart = new Chart(_grammar);
            var firstProduction = _grammar.Productions[0];
            var startProductions = _grammar.Productions.Where(p => p.LeftHandSide.Equals(firstProduction.LeftHandSide));

            foreach (var startProduction in startProductions)
            {
                var startState = new State(startProduction, 0, 0);
                chart.EnqueueAt(0, startState);
                Console.WriteLine("{0}\t{1}\t # Start", 0, startState);
            }

            int origin = 0;
            // TODO:
            // Update algorithm to follow these steps from :
            // https://github.com/jeffreykegler/kollos/blob/master/notes/misc/leo2.md
            // * you first perform all scans;
            // * then all completions;
            // * then all predictions;
            // * and finally do post-processing, including eager computation of Leo items.
            foreach (var token in tokens)
            {                 
                for (int c = 0; c < chart[origin].Count; c++)
                {
                    var state = chart[origin][c];
                    if (state.StateType == StateType.Transitive)
                        continue;
                    if (!state.IsComplete())
                    {
                        if (state.CurrentSymbol().SymbolType == SymbolType.NonTerminal)
                        {
                            Predict(state, origin, chart);
                        }
                        else
                        {
                            Scan(state, origin, chart, token);
                        }
                    }
                    else
                    {
                        Complete(state, origin, chart);
                    }
                }
                origin++;
            }
            return chart;
        }

        private void Predict(IState predict, int j, Chart chart)
        {
            var currentSymbol = predict.CurrentSymbol();
            foreach (var production in _grammar.RulesFor(currentSymbol))
            {
                var state = new State(production, 0, j);
                chart.EnqueueAt(j, state);
                Log("Predict", j, state);
            }
        }

        private void Scan(IState scan, int j, Chart chart, char token)
        {
            int i = scan.Origin;
            for (var s = 0; s < chart[j].Count; s++)
            {
                var state = chart[j][s];
                if (state.StateType == StateType.Transitive)
                    continue;
                if (!state.IsComplete() && state.CurrentSymbol().Value == token.ToString())
                {
                    var scanState = new State(
                        state.Production,
                        state.Position + 1,
                        i);
                    chart.EnqueueAt(j + 1, scanState);
                    LogScan(j+1, scanState, token);
                }
            }
        }

        private void LogScan(int origin, IState state, char token)
        {
            Console.Write("{0}\t{1}", origin, state);
            Console.WriteLine("\t # Scan {0}", token);
        }

        private void Log(string operation, int origin, IState state)
        {
            Console.Write("{0}\t{1}", origin, state);
            Console.WriteLine("\t # {0}", operation);
        }

        private void Complete(IState completed, int k, Chart chart)
        {
            var searchSymbol = completed.Production.LeftHandSide;
            OptimizeReductionPath(searchSymbol, k, chart);
            var transitiveState = FindTransitiveState(chart[k], searchSymbol);
            if (transitiveState != null)
            {
                var topmostItem = new State(transitiveState.Production, transitiveState.Position, transitiveState.Origin);
                chart.EnqueueAt(k, topmostItem);
            }
            else
            {
                int j = completed.Origin;
                for (int s = 0; s < chart[j].Count; s++)
                {
                    var state = chart[j][s];
                    if (IsDerivedState(completed.Production.LeftHandSide, state))
                    {
                        int i = state.Origin;
                        var nextState = new State(state.Production, state.Position + 1, i);
                        chart.EnqueueAt(k, nextState);
                        Log("Complete", k, nextState);
                    }
                }
            }
        }

        private void OptimizeReductionPath(ISymbol searchSymbol, int k, Chart chart)
        {
            IState t_rule = null;
            OptimizeReductionPathRecursive(searchSymbol, k, chart, ref t_rule);
        }

        private void OptimizeReductionPathRecursive(ISymbol searchSymbol, int k, Chart chart, ref IState t_rule)
        {
            var list = chart[k];
            var transitiveState = FindTransitiveState(list, searchSymbol);
            if (transitiveState != null)
            {
                t_rule = transitiveState;
            }
            else 
            {
                var derivedState = FindDerivedState(list, searchSymbol);
                if (derivedState != null)
                {
                    Console.WriteLine("Found {0}", derivedState);
                    t_rule = derivedState.IsComplete() 
                        ? derivedState
                        : new State(derivedState.Production, derivedState.Position + 1, derivedState.Origin);
                    OptimizeReductionPathRecursive(derivedState.Production.LeftHandSide, derivedState.Origin, chart, ref t_rule);
                    if (t_rule != null)
                    {
                        var transitionItem = new TransitionState(
                            searchSymbol,
                            t_rule.Production,
                            t_rule.Production.RightHandSide.Count,
                            t_rule.Origin);
                        chart.EnqueueAt(k, transitionItem);
                    }
                }
            }
        }

        IState FindDerivedState(IReadOnlyList<IState> list, ISymbol searchSymbol)
        {
            var derivedItemCount = 0;
            IState derivedItem = null;
            for (int s = 0; s < list.Count; s++)
            {
                var state = list[s];
                if (IsDerivedState(searchSymbol, state))
                {
                    bool moreThanOneDerivedItemExists = derivedItemCount > 0;
                    if (moreThanOneDerivedItemExists)
                        return null;
                    derivedItemCount++;
                    derivedItem = state;
                }
            }
            return derivedItem;
        }
        
        private bool IsDerivedState(ISymbol searchSymbol, IState state)
        {
            if (state.IsComplete())
                return false;
            return state.CurrentSymbol().Equals(searchSymbol);
        }

        private IState FindTransitiveState(IReadOnlyList<IState> list, ISymbol searchSymbol)
        {
            for (int s = 0; s < list.Count; s++)
            {
                var state = list[s];
                if (IsTransitiveState(searchSymbol, state))
                    return state;
            }
            return null;
        }

        private bool IsTransitiveState(ISymbol searchSymbol, IState state)
        {
            if (state.StateType != StateType.Transitive)
                return false;
            var transitiveState = state as TransitionState;
            return transitiveState.Recognized.Equals(searchSymbol);
        }
    }
}
