namespace Pliant.Forest
{
    public class SinglePassForestTraversalPathProvider : IForestTraversalPathProvider
    {
        SinglePassForestTraversalPath _current;
        SinglePassForestTraversalPathProviderState _state;

        enum SinglePassForestTraversalPathProviderState
        {
            New,
            Current,
            Done
        }

        public SinglePassForestTraversalPathProvider()
        {
            _state = SinglePassForestTraversalPathProviderState.New;
            _current = new SinglePassForestTraversalPath();
        }

        public IForestTraversalPath Current
        {
            get
            {
                switch (_state)
                {
                    case SinglePassForestTraversalPathProviderState.Current:
                        return _current;
                    default:
                        return null;
                }
            }
        }

        public bool MoveNext()
        {
            switch (_state)
            {
                case SinglePassForestTraversalPathProviderState.Done:
                    return false;

                case SinglePassForestTraversalPathProviderState.New:
                    _state = SinglePassForestTraversalPathProviderState.Current;
                    return true;

                case SinglePassForestTraversalPathProviderState.Current:
                    _state = SinglePassForestTraversalPathProviderState.Done;
                    return false;
            }
            return false;
        }
    }
}
