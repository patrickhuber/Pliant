using System;
namespace Pliant
{
    public interface ITerminal : ISymbol
    {
        bool IsMatch(char character);
    }
}
