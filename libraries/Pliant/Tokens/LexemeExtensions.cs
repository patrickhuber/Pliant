using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pliant.Runtime;

namespace Pliant.Tokens
{
    public static class LexemeExtensions
    {
        public static bool Scan(this ILexeme lexeme, char c)
        {
            return lexeme.Scan(new ParseContext(), c);
        }
    }
}
