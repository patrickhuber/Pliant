using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface ILexeme
    {
        ILexerRule LexerRule { get; }
        string Capture { get; }
        bool Scan(char c);
        bool IsAccepted();
    }
}
