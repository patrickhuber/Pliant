namespace Pliant
{
    public interface INullable<T>
    {
        T Value { get; }
        bool HasValue { get; }
    }
}