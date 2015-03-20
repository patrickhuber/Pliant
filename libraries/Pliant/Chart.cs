using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Chart
    {
        private IDictionary<int, IList<IState>> _body;
        private ReadWriteList<IEarleme> _earlemes;

        public Chart()
        {
            _body = new Dictionary<int, IList<IState>>();
            _earlemes = new ReadWriteList<IEarleme>();
        }

        public bool Enqueue(int index, IState state)
        {
            if (!_body.ContainsKey(index))
                _body.Add(index, new List<IState>());
            var list = _body[index];
            if (list.Any(x => x.Equals(state) 
                && x.StateType == state.StateType))
                return false;
            list.Add(state);
            EnqueueAndClassify(index, state);
            return true;
        }

        private bool EnqueueAndClassify(int index, IState state)
        {
            if (_earlemes.Count <= index)
                _earlemes.Add(new Earleme());
            var earleme = _earlemes[index];
            return earleme.Enqueue(state);
        }
        
        public IReadOnlyList<IState> this[int index]
        {
            get
            {
                return new ReadOnlyList<IState>(_body[index]);
            }
        }

        public IList<IEarleme> Earlemes { get { return _earlemes; } }

        public int Count
        {
            get { return _body.Count; }
        }
    }
}
