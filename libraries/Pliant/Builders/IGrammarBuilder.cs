using Pliant.Grammars;
using System;
namespace Pliant.Builders
{
    public interface IGrammarBuilder
    {
        IGrammar ToGrammar();
    }
}
