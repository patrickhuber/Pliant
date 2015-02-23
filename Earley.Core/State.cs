using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class State : Earley.IState
    {
        private int _count = -1;

        public IProduction Production { get; private set; }
        
        public int Position { get; private set; }
        
        public int Origin { get; private set; }

        public State(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, "production");
            Assert.IsGreaterThanZero(position, "position");
            Assert.IsGreaterThanZero(origin, "origin");
            Production = production;
            Position = position;
            Origin = origin;
        }

        public bool IsComplete()
        {
            // cache the count because productions are immutable
            if (_count < 0)
                _count = Production.RightHandSide.Count();
            return _count <= Position;
        }

        public ISymbol CurrentSymbol()
        {
            if (IsComplete())
                return null;
            return Production.RightHandSide[Position];
        }
        
        public override bool Equals(object obj)
        {
            var state = obj as State;
            if (state == null)
                return false;
            return Position == state.Position
                && Origin == state.Origin
                && Production.Equals(state.Production);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode()
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
                    p == Position ? "." : " " , 
                    Production.RightHandSide[p]);
            }
            
            if (Position == Production.RightHandSide.Count)
                stringBuilder.Append(".");

            stringBuilder.AppendFormat("\t\t({0})", Origin);
            return stringBuilder.ToString();
        }

        public virtual StateType StateType { get { return Earley.StateType.Normal; } }
    }
}
