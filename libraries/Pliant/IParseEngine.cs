using Pliant.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
