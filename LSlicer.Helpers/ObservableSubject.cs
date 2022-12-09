using System;
using System.Linq;

namespace LSlicer.Helpers
{
    public class ObservableSubject<T> : IObservable<T>, INotifier<T>
    {
        private readonly ConcurrentHashSet<IObserver<T>> _observers;

        public ObservableSubject()
        {
            _observers = new ConcurrentHashSet<IObserver<T>>();
        }

        public bool IsEmpty { get { return _observers.Count == 0; } }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_observers.Add(observer))
                return new Unsubscriber<T>(_observers.ToList(), observer);
            throw new Exception("Unable to subscribe.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            foreach (var obsr in _observers)
                ((IObserver<T>)obsr).OnCompleted();
            _observers.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Notify(T state)
        {
            foreach (var obsr in _observers)
                ((IObserver<T>)obsr).OnNext(state);
        }

        public virtual void Notify(Exception ex)
        {
            foreach (var obsr in _observers)
                ((IObserver<T>)obsr).OnError(ex);
        }
    }
}
