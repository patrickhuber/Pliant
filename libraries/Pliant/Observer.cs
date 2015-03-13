using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Observer<T> : IObserver<T>
    {
        private Action<T> _onNext;
        private Action<Exception> _onError;
        private Action _onCompleted;

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
