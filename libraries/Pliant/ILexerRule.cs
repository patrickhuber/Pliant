using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface ILexerRule
    {
        bool Greedy { get; }
        IGrammar Grammar { get; }
    }
}
