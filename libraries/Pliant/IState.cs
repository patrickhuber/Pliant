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
        IDottedRule DottedRule { get; }
    }
}
