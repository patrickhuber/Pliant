using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public interface IState
    {
        IProduction Production { get; }

        int Origin { get; }

        StateType StateType { get; }

        ISymbol PreDotSymbol { get; }

        ISymbol PostDotSymbol { get; }

        int Position { get; }

        bool IsComplete { get; }

        IState NextState();

        IState NextState(int newOrigin);

        IState NextState(IForestNode parseNode);

        IState NextState(int newOrigin, IForestNode parseNode);

        IForestNode ParseNode { get; set; }

        bool IsSource(ISymbol searchSymbol);
    }
}