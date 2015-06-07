using Pliant.Grammars;
using Pliant.Nodes;
using System.Text;

namespace Pliant.Charts
{
    public class State : IState
    {
        public IProduction Production { get; private set; }

        public int Origin { get; private set; }
        
        public IDottedRule DottedRule { get; private set; }
        
        public virtual StateType StateType { get { return StateType.Normal; } }

        public INode ParseNode { get; set; }
        
        public State(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, "production");
            Assert.IsGreaterThanEqualToZero(position, "position");
            Assert.IsGreaterThanEqualToZero(origin, "origin");
            Production = production;
            Origin = origin;
            DottedRule = new DottedRule(production, position);
        }

        public State(IProduction production, int position, int origin, INode parseNode)
            : this(production, position, origin)
        {
            ParseNode = parseNode;
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
            const string Dot = "\u25CF";

            for (int p = 0; p < Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == DottedRule.Position ? Dot : " ",
                    Production.RightHandSide[p]);
            }
            
            if (DottedRule.Position == Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            stringBuilder.AppendFormat("\t\t({0})", Origin);
            return stringBuilder.ToString();
        }
        
        public IState NextState()
        {
            return NextState(null as INode);
        }

        public IState NextState(INode node)
        {
            if (DottedRule.IsComplete)
                return null;
            return new State(
                Production,
                DottedRule.Position + 1,
                Origin,
                node);
        }

        public IState NextState(int newOrigin)
        {
            return NextState(newOrigin, null);
        }

        public IState NextState(int newOrigin, INode parseNode)
        {
            if (DottedRule.IsComplete)
                return null;
            return new State(
                Production,
                DottedRule.Position + 1,
                newOrigin,
                parseNode);
        }
        
        public bool IsSource(ISymbol searchSymbol)
        {
            if (DottedRule.IsComplete)
                return false;
            return DottedRule.PostDotSymbol.Value.Equals(searchSymbol);
        }                
    }
}
