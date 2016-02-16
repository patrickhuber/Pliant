using System;
using System.Collections.Generic;

namespace Pliant
{
    public abstract class Observable<T> : IObservable<T>
    {
        private IList<IObserver<T>> _observers;

        protected Observable()
        {
            _observers = new List<IObserver<T>>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        protected virtual void OnNext(T value)
        {
            foreach (var observer in _observers)
                observer.OnNext(value);
        }

        protected virtual void OnError(Exception exception)
        {
            foreach (var observer in _observers)
                observer.OnError(exception);
        }

        protected virtual void Complete()
        {
            foreach (var observer in _observers)
                if (_observers.Contains(observer))
                    observer.OnCompleted();
            _observers.Clear();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly IList<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public Unsubscriber(IList<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}