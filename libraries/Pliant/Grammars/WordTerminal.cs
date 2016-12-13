using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Grammars
{
    public class WordTerminal : BaseTerminal
    {
        private static readonly Interval[] _intervals = 
        {
            new Interval('A', 'Z'),
            new Interval('a', 'z'),
            new Interval('0', '9'),
            new Interval('_', '_')
        };
        
        public override IReadOnlyList<Interval> GetIntervals()
        {
            return _intervals;
        }

        public override bool IsMatch(char character)
        {
            return ('A' <= character && character <= 'Z'
                || 'a' <= character && character <= 'z'
                || '0' <= character && character <= '9'
                || '_' == character) ;                
        }
    }
}
