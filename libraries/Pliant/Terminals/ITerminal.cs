using System;
namespace Pliant.Terminals
{
    public interface ITerminal : ISymbol
    {
        bool IsMatch(char character);
    }
}
