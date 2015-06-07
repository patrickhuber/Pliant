using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tokens
{
    public interface IToken
    {
        string Value { get; }
        int Origin { get; }
        TokenType TokenType { get; }
    }
}
