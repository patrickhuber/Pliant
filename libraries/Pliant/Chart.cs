using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Chart
    {
        private ReadWriteList<IEarleme> _earlemes;

        public Chart()
        {
            _earlemes = new ReadWriteList<IEarleme>();
        }
        
        public bool Enqueue(int index, IState state)
        {
            if (_earlemes.Count <= index)
                _earlemes.Add(new Earleme());
            var earleme = _earlemes[index];
            return earleme.Enqueue(state);
        }
        
        public IReadOnlyList<IEarleme> Earlemes { get { return _earlemes; } }

        public int Count
        {
            get { return Earlemes.Count; }
        }
    }
}
