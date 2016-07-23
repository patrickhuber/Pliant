using System;

namespace Pliant.Forest
{
    public class MultiplePassForestTraversalPath : IForestTraversalPath
    {
        public int AmbiguityIndex { get; private set; }

        private int _currentIndex;

        public MultiplePassForestTraversalPath(int ambiguityIndex)
        {
            AmbiguityIndex = ambiguityIndex;
            _currentIndex = 0;
        }

        public IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode)
        {
            throw new NotImplementedException();
        }
    }
}
