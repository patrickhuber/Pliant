using System;
namespace Earley
{
    public interface ITerminal : ISymbol
    {
        bool IsMatch(char character);
    }
}
