using Pliant.Nodes;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant
{
    public interface IParseEngine
    {
        void Reset();
        bool IsAccepted();
        INode GetParseForest();
        IEnumerable<ILexerRule> GetExpectedLexerRules();
        bool Pulse(IToken token);
        IGrammar Grammar { get; }
        int Location { get; }
    }
}
