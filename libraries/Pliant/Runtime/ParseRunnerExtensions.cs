using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Runtime
{
    public static class ParseRunnerExtensions
    {
        public static bool Read(this IParseRunner parseRunner)
        {
            return parseRunner.Read(new ParseContext());
        }
    }
}
