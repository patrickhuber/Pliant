using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class State : IState
    {
        public IProduction Production { get; private set; }

        public int Origin { get; private set; }

        public State(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, "production");
            Assert.IsGreaterThanZero(position, "position");
            Assert.IsGreaterThanZero(origin, "origin");
            Production = production;
            Origin = origin;
            DottedRule = new DottedRule(production, position);
        }
                
        public override bool Equals(object obj)
        {
            var state = obj as State;
            if (state == null)
                return false;
            return DottedRule.Position == state.DottedRule.Position
                && Origin == state.Origin
                && Production.Equals(state.Production);
        }

        public override int GetHashCode()
        {
            return DottedRule.Position.GetHashCode()
                ^ Origin.GetHashCode()
                ^ Production.GetHashCode();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", Production.LeftHandSide.Value);
            
            int p = 0;
            for (p=0; p < Production.RightHandSide.Count; p++)
            {       
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == DottedRule.Position ? "\u25CF" : " ", 
                    Production.RightHandSide[p]);
            }
            
            if (DottedRule.Position == Production.RightHandSide.Count)
                stringBuilder.Append("\u25CF");

            stringBuilder.AppendFormat("\t\t({0})", Origin);
            return stringBuilder.ToString();
        }

        public virtual StateType StateType { get { return StateType.Normal; } }


        public IState NextState()
        {
            if (DottedRule.IsComplete)
                return null;
            return new State(Production, DottedRule.Position + 1, Origin);
        }

        public IState NextState(int newOrigin)
        {
            if (DottedRule.IsComplete)
                return null;
            return new State(
                Production, 
                DottedRule.Position + 1, 
                newOrigin);
        }
        
        public bool IsSource(ISymbol searchSymbol)
        {
            if (DottedRule.IsComplete)
                return false;
            return DottedRule.Symbol.Equals(searchSymbol);
        }

        public IDottedRule DottedRule { get; private set; }
    }
}
