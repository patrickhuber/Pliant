using System;
namespace Pliant.Grammars
{
    public interface ITerminal : ISymbol
    {
        bool IsMatch(char character);
    }
}
