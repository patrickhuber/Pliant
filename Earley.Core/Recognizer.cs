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
            var startState = new State(_grammar.Productions[0], 0, 0);
            chart.EnqueueAt(0, startState);

            Console.Write("{0}\t{1}", 0, startState);
            Console.WriteLine("\t # Start");

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
                Console.Write("{0}\t{1}", j, state);
                Console.WriteLine("\t # Predict");
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
                    Console.Write("{0}\t{1}", j, scanState);
                    Console.WriteLine("\t # Scan {0}", token);
                }
            }
        }

        private void Complete(IState completed, int k, Chart chart)
        {
            // TODO: Do Leo Optimization Step Here
            int j = completed.Origin;
            for (int s = 0; s < chart[j].Count; s++)
            {
                var state = chart[j][s];
                if (IsDerivedState(completed, state))
                {
                    int i = state.Origin;
                    var nextState = new State(state.Production, state.Position + 1, i);
                    chart.EnqueueAt(k, nextState);
                    Console.Write("{0}\t{1}", k, nextState);
                    Console.WriteLine("\t # Complete");
                }
            }
        }

        void OptimizeReductionPath(IState state, int k, Chart chart)
        {
            IState t_rule = null;
            OptimizeReductionPathRecursive(state, k, chart, ref t_rule);
        }

        void OptimizeReductionPathRecursive(IState completed, int k, Chart chart, ref IState t_rule)
        {
            var list = chart[k];
            var transitiveState = FindTransitiveState(list, completed);
            if (transitiveState != null)
            {
                t_rule = transitiveState;
            }
            else 
            {
                var derivedState = FindDerivedState(list, completed);
                if (derivedState != null)
                {
                    t_rule = derivedState;
                    OptimizeReductionPathRecursive(derivedState, derivedState.Origin, chart, ref t_rule);
                    if (t_rule != null)
                    {
                        var transitionItem = new TransitionState(
                            completed.Production.LeftHandSide,
                            t_rule.Production,
                            t_rule.Production.RightHandSide.Count,
                            t_rule.Origin);
                        chart.EnqueueAt(k, transitionItem);
                    }
                }
            }
        }

        IState FindDerivedState(IReadOnlyList<IState> list, IState currentState)
        {
            var derivedItemCount = 0;
            IState derivedItem = null;
            for (int s = 0; s < list.Count; s++)
            {
                var state = list[s];
                if (IsDerivedState(currentState, state))
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
        
        private bool IsDerivedState(IState completed, IState state)
        {
            if (state.IsComplete())
                return false;
            return state.CurrentSymbol().Equals(completed.Production.LeftHandSide);
        }

        private IState FindTransitiveState(IReadOnlyList<IState> list, IState source)
        {
            for (int s = 0; s < list.Count; s++)
            {
                var state = list[s];
                if (IsTransitiveState(source, state))
                    return state;
            }
            return null;
        }

        private bool IsTransitiveState(IState completed, IState state)
        {
            if (state.StateType != StateType.Transitive)
                return false;
            var transitiveState = state as TransitionState;
            return transitiveState.Recognized.Equals(completed.Production.LeftHandSide);
        }
    }
}
