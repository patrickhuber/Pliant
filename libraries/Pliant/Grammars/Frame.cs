using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    internal class Frame
    {
        public SortedSet<PreComputedState> Data { get; private set; }
        public Dictionary<ISymbol, Frame> Transitions { get; private set; }
        public Frame NullTransition { get; set; }

        public Frame(SortedSet<PreComputedState> data)
        {
            Data = data;
            Transitions = new Dictionary<ISymbol, Frame>();
            _hashCode = ComputeHashCode(data);
        }

        private readonly int _hashCode;

        public void AddTransistion(ISymbol symbol, Frame target)
        {
            Frame value = null;
            if (!Transitions.TryGetValue(symbol, out value))
                Transitions.Add(symbol, target);
        }

        static int ComputeHashCode(SortedSet<PreComputedState> data)
        {
            return HashCode.Compute(data);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;

            var frame = obj as Frame;
            if (((object)frame) == null)
                return false;

            foreach (var item in Data)
                if (!frame.Data.Contains(item))
                    return false;

            return true;
        }
    }
}
