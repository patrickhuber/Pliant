namespace Pliant.Forest
{
    public class Accumulator
    {
        private const int Seed = 0;

        public int Value { get; private set; }

        public Accumulator()
        {
            Reset();
        }

        public void Increment()
        {
            Value++;
        }

        public void Reset()
        {
            Value = Seed;
        }
    }
}
