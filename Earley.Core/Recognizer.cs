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
            foreach (var token in tokens)
            {                 
                for (int c = 0; c < chart[origin].Count; c++)
                {
                    var state = chart[origin][c];
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
            for (var s = 0; s < chart[j].Count;s++)
            {
                var state = chart[j][s];
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
            int j = completed.Origin;
            for (int s = 0; s < chart[j].Count; s++)
            {
                var state = chart[j][s];
                var stateSymbol = state.CurrentSymbol();
                if (stateSymbol != null && stateSymbol.Value == completed.Production.LeftHandSide.Value)
                {
                    int i = state.Origin;
                    var nextState = new State(state.Production, state.Position + 1, i);
                    chart.EnqueueAt(k, nextState);
                    Console.Write("{0}\t{1}", k, nextState);
                    Console.WriteLine("\t # Complete");
                }
            }
        }
    }
}
