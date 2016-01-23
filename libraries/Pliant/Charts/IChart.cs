namespace Pliant.Charts
{
    public interface IChart : IReadOnlyChart
    {
        bool Enqueue(int index, IState state);
    }
}