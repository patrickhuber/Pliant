using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class LeoReducer
    {
        private Chart _chart;
        private int _position;
        private Production _rule;

        public LeoReducer(Chart chart, int position, Production rule)
        {
            _chart = chart;
            _position = position;
            _rule = rule;
        }

        public void Update()
        {
            
        }

        private bool ContainsTransitiveItem(IReadOnlyCollection<IState> _states, IProduction production)
        {
 
        }
    }
}
