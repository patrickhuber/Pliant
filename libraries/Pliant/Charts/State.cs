using Pliant.Collections;
using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Charts
{
    public class State : IState
    {
        public IProduction Production { get; private set; }

        public int Origin { get; private set; }

        public ISymbol PreDotSymbol { get; private set; }

        public ISymbol PostDotSymbol { get; private set; }

        public int Position { get; private set; }

        public virtual StateType StateType { get { return StateType.Normal; } }

        public IForestNode ParseNode { get; set; }

        public IReadOnlyList<IState> Parents { get { return _parents; } }

        private readonly int _hashCode;
        private UniqueList<IState> _parents;
        
        public State(IProduction production, int position, int origin)
        {
            Assert.IsNotNull(production, nameof(production));
            Assert.IsGreaterThanEqualToZero(position, nameof(position));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            Production = production;
            Origin = origin;
            Position = position;
            PostDotSymbol = GetPostDotSymbol(position, production);
            PreDotSymbol = GetPreDotSymbol(position, production);
            _hashCode = ComputeHashCode();
        }
        
        public IState NextState()
        {
            if (IsComplete)
                return null;
            var state = new State(
                Production,
                Position + 1,
                Origin);
            return state;
        }
        
        public bool IsComplete
        {
            get { return Position == Production.RightHandSide.Count; }
        }

        public bool IsSource(ISymbol searchSymbol)
        {
            if (IsComplete)
                return false;
            return PostDotSymbol.Equals(searchSymbol);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var state = obj as State;
            if (state == null)
                return false;
            // PERF: Hash Codes are Cached, so equality performance is cached as well
            return GetHashCode() == state.GetHashCode();
        }


        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Position.GetHashCode(),
                Origin.GetHashCode(),
                Production.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
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
                    p == Position ? Dot : " ",
                    Production.RightHandSide[p]);
            }

            if (Position == Production.RightHandSide.Count)
                stringBuilder.Append(Dot);

            stringBuilder.Append($"\t\t({Origin})");
            return stringBuilder.ToString();
        }

        private static ISymbol GetPreDotSymbol(int position, IProduction production)
        {
            if (position == 0 || production.IsEmpty)
                return null;
            return production.RightHandSide[position - 1];
        }

        private static ISymbol GetPostDotSymbol(int position, IProduction production)
        {
            if (position >= production.RightHandSide.Count)
                return null;
            return production.RightHandSide[position];
        }

        public void Merge(IState state)
        {
            // the states must have the same hashcode
            if (!_hashCode.Equals(state.GetHashCode()))
                return;

            for (int i = 0; i < state.Parents.Count; i++)
                _parents.Add(state.Parents[i]);
        }
    }
}