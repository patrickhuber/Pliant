using Pliant.Grammars;

namespace Pliant.Charts
{
    public class CachedStateFrameTransition
    {
        public ISymbol Symbol { get; private set; }
        public Frame Frame { get; private set; }
        public int Origin { get;  private set;}

        public CachedStateFrameTransition(ISymbol symbol, Frame frame, int origin)
        {
            Symbol = symbol;
            Frame = frame;
            Origin = origin;
        }
    }
}