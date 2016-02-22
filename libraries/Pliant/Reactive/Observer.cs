using System;

namespace Pliant
{
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        public Observer(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnCompleted()
        {
            if (_onCompleted != null)
                _onCompleted();
        }

        public void OnError(Exception error)
        {
            if (_onError != null)
                _onError(error);
        }

        public void OnNext(T value)
        {
            if (_onNext != null)
                _onNext(value);
        }
    }
}