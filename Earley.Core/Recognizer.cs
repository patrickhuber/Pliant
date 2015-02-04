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

        private class RecognizerContext
        {
            public Chart Chart { get; set; }
            public IState State { get; set; }
            public int Origin { get; set; }
            public char Token { get; set; }
        }

        public Chart Parse(IEnumerable<char> tokens)
        {
            var chart = new Chart(_grammar);
            chart.EnqueueAt(0, new State(_grammar.Productions[0], 0, 0));
            
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
            var addedProductions = new List<IProduction>();
            foreach (var production in _grammar.RulesFor(currentSymbol))
            {
                var state = new State(production, 0, j);
                chart.EnqueueAt(j, state);
            }
        }

        private void Scan(IState scan, int j, Chart chart, char token)
        {
            int i = scan.Origin;
            foreach (var state in chart[j])
            {
                if(!state.IsComplete() && state.CurrentSymbol().Value == token.ToString())
                {
                    var production = new Production(
                        state.Production.LeftHandSide, 
                        new Symbol(SymbolType.Terminal, token.ToString()));
                    var scanState = new State(production, 1, i);
                    chart.EnqueueAt(j + 1, scanState);
                }
            }
        }

        private void Complete(IState completed, int k, Chart chart)
        {
            int j = completed.Origin;
            foreach (var state in chart[j])
            {   
                var stateSymbol = state.CurrentSymbol();
                if (stateSymbol != null && stateSymbol.Value == completed.Production.LeftHandSide.Value)
                {
                    int i = state.Origin;
                    var nextState = new State(state.Production,  state.Position + 1, i);
                    chart.EnqueueAt(k, nextState);
                }
            }
        }
    }
}
