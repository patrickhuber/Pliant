using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Token : IToken
    {
        public StringBuilder Value { get; private set; }

        public int Origin { get; private set; }

        public TokenType TokenType { get; private set; }

        public Token(StringBuilder value, int origin, TokenType tokenType)
        {
            Value = value;
            Origin = origin;
            TokenType = tokenType;
        }
    }
}
