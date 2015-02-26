using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Chart
    {
        private IGrammar _grammar;
        private IDictionary<int, List<IState>> _body;
        
        public Chart(IGrammar grammar)
        {
            Assert.IsNotNull(grammar, "grammar");
            _grammar = grammar;
            _body = new Dictionary<int, List<IState>>();
        }

        public bool EnqueueAt(int index, IState state)
        {
            if (!_body.ContainsKey(index))
                _body.Add(index, new List<IState>());
            var list = _body[index];
            if (list.Any(x => x.Equals(state) 
                && x.StateType == state.StateType))
                return false;
            list.Add(state);
            return true;
        }
        
        public IReadOnlyList<IState> this[int index]
        {
            get
            {
                return new ReadOnlyList<IState>(_body[index]);
            }
        }

        public int Count
        {
            get { return _body.Count; }
        }
    }
}
