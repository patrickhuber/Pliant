using System;
using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class Chart : IChart
    {
        private List<EarleySet> _earleySets;
        
        public IReadOnlyList<IEarleySet> EarleySets { get { return _earleySets; } }

        public Chart()
        {
            _earleySets = new List<EarleySet>();
        }

        public bool Enqueue(int index, IState state)
        {
            IEarleySet earleySet = GetEarleySet(index);
            return earleySet.Enqueue(state);
        }

        public int Count
        {
            get { return EarleySets.Count; }
        }

        public bool Contains(int index, StateType stateType, IDottedRule dottedRule, int origin)
        {
            var earleySet = GetEarleySet(index);
            return earleySet.Contains(stateType, dottedRule, origin);
        }

        private EarleySet GetEarleySet(int index)
        {
            EarleySet earleySet = null;
            if (_earleySets.Count <= index)
            {
                earleySet = new EarleySet(index);
                _earleySets.Add(earleySet);
            }
            else
            {
                earleySet = _earleySets[index];
            }

            return earleySet;
        }
    }
}