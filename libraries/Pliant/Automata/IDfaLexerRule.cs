using Pliant.Grammars;

namespace Pliant.Automata
{
    public interface IDfaLexerRule : ILexerRule
    {
        IDfaState Start { get; }
    }
}