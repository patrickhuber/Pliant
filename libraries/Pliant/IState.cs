using System;
namespace Pliant
{
    public interface IState
    {
        IProduction Production { get; }
        int Origin { get; }
        StateType StateType { get; }
        IState NextState();
        IState NextState(int newOrigin);
        IState NextState(INode parseNode);
        IState NextState(int newOrigin, INode parseNode);
        IDottedRule DottedRule { get; }
        INode ParseNode { get; set; }
        bool IsSource(ISymbol searchSymbol);
    }
}
