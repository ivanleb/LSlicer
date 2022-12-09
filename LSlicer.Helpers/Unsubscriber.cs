using System;
using System.Collections.Generic;

namespace LSlicer.Helpers
{
    public class Unsubscriber<T> : IDisposable
    {
        private ConcurrentHashSet<IObserver<T>> _observers;
        private IObserver<T> _observer;

        public Unsubscriber(IList<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = new ConcurrentHashSet<IObserver<T>>( observers);
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
