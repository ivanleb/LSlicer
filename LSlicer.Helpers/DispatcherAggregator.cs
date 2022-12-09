using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public class DispatcherAggregator<T>
    {
        IList<IDispatcher<T>> _dispatchers;
        IList<WeakReference<IObserver<T>>> _observers;

        public DispatcherAggregator()
        {
            _dispatchers = new List<IDispatcher<T>>();
            _observers = new List<WeakReference<IObserver<T>>>();
        }

        public void Subscribe(IObserver<T> observer)
        {
            foreach (var dispatcher in _dispatchers)
                dispatcher.Subscribe(observer);
            _observers.Add(new WeakReference<IObserver<T>>(observer));
        }

        public void AddDispatcher(IDispatcher<T> dispatcher)
        {
            _dispatchers.Add(dispatcher);
            foreach (var observer in _observers)
            {
                IObserver<T> obsrv;
                if (observer.TryGetTarget(out obsrv))
                {
                    dispatcher.Subscribe(obsrv);
                }
            }
        }

        public void RemoveDispatcher(IDispatcher<T> dispatcher)
        {
            _dispatchers.Remove(dispatcher);
        }

        public void Push(T state)
        {
            foreach (var dispatcher in _dispatchers)
                dispatcher.Push(state);
        }

        public void Dispatch(TimeSpan period)
        {
            foreach (var dispatcher in _dispatchers)
                dispatcher.Dispatch(period);
        }

        public void StopDispatch()
        {
            foreach (var dispatcher in _dispatchers)
                dispatcher.StopDispatch();
        }
    }
}
