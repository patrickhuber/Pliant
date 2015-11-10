using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.RegularExpressions
{
    public class Regex
    {
        public bool StartsWith { get; set; }
        public RegexExpression Expression { get; set; }
        public bool EndsWith { get; set; }
    }
}
