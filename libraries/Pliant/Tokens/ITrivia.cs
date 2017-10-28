using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tokens
{
    public interface ITrivia
    {
        int Position { get; }
        string Value { get; }
        TokenType TokenType { get; }
    }
}
