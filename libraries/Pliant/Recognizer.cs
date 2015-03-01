using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Recognizer
    {
        private IGrammar _grammar;

        public Recognizer(IGrammar grammar)
        {
            Assert.IsNotNull(grammar, "grammar");
            _grammar = grammar;
        }

        public Chart Parse(TextReader textReader)
        {
            var chart = new Chart(_grammar);
            
            foreach (var startProduction in _grammar.StartProductions())
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
            while(origin < chart.Count)
            {
                var token = (char)textReader.Read();
                for (int c = 0; c < chart[origin].Count; c++)
                {
                    var state = chart[origin][c];
                    if (state.StateType == StateType.Transitive)
                        continue;
                    if (!state.IsComplete())
                    {
                        var currentSymbol = state.CurrentSymbol();
                        if (currentSymbol.SymbolType == SymbolType.NonTerminal)
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

        private void Predict(IState sourceState, int j, Chart chart)
        {
            var nonTerminal = sourceState.CurrentSymbol() as INonTerminal;
            foreach (var production in _grammar.RulesFor(nonTerminal))
            {
                var state = new State(production, 0, j);
                if(chart.EnqueueAt(j, state))
                    Log("Predict", j, state);

                var stateIsNullable = state.Production.RightHandSide.Count == 0;
                if (stateIsNullable)
                {
                    var aycockHorspoolState = new State(sourceState.Production, sourceState.Position + 1, j);
                    chart.EnqueueAt(j, aycockHorspoolState);
                    Log("Predict", j, aycockHorspoolState);
                }
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
                if (!state.IsComplete() )
                {
                    var currentSymbol = state.CurrentSymbol();
                    if (currentSymbol.SymbolType == SymbolType.Terminal)
                    {
                        var terminal = currentSymbol as Terminal;
                        if (terminal.IsMatch(token))
                        {
                            var scanState = new State(
                                state.Production,
                                state.Position + 1,
                                i);
                            if(chart.EnqueueAt(j + 1, scanState))
                                LogScan(j + 1, scanState, token);
                        }
                    }
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
                if(chart.EnqueueAt(k, topmostItem))
                    Log("Complete", k, topmostItem);
            }
            else
            {
                int j = completed.Origin;
                for (int s = 0; s < chart[j].Count; s++)
                {
                    var state = chart[j][s];
                    if (IsSourceStateState(completed.Production.LeftHandSide, state))
                    {
                        int i = state.Origin;
                        var nextState = new State(state.Production, state.Position + 1, i);
                        if(chart.EnqueueAt(k, nextState))
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
                var sourceState = FindSourceState(list, searchSymbol);

                if (sourceState != null)
                {
                    var sourceStateNext = sourceState.NextState();
                    if (sourceStateNext.IsComplete())
                    {
                        t_rule = sourceStateNext;
                        OptimizeReductionPathRecursive(sourceState.Production.LeftHandSide, sourceState.Origin, chart, ref t_rule);
                        if (t_rule != null)
                        {
                            var transitionItem = new TransitionState(
                                searchSymbol,
                                t_rule.Production,
                                t_rule.Production.RightHandSide.Count,
                                t_rule.Origin);
                            if (chart.EnqueueAt(k, transitionItem))
                                Log("Transition", k, transitionItem);
                        }
                    }
                }
            }
        }

        IState FindSourceState(IReadOnlyList<IState> list, ISymbol searchSymbol)
        {
            var sourceItemCount = 0;
            IState sourceItem = null;
            for (int s = 0; s < list.Count; s++)
            {
                var state = list[s];
                if (IsSourceStateState(searchSymbol, state))
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
        
        private bool IsSourceStateState(ISymbol searchSymbol, IState state)
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
