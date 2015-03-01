using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Lexeme : ILexeme
    {
        public INonTerminal LeftHandSide { get; private set; }

        public IReadOnlyList<ITerminal> Terminals { get; private set; }

        public Lexeme(INonTerminal leftHandSide, params ITerminal[] terminals)
        {
            LeftHandSide = leftHandSide;
            Terminals = new ReadOnlyList<ITerminal>(
                new List<ITerminal>(
                    terminals ?? new ITerminal[]{}));
        }
    }
}
