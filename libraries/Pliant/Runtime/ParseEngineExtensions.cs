using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pliant.Tokens;

namespace Pliant.Runtime
{
    public static class ParseEngineExtensions
    {
        public static bool Pulse(this IParseEngine parseEngine, IToken token)
        {
            return parseEngine.Pulse(new ParseContext(), token);
        }
    }
}
