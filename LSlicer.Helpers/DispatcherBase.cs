using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public abstract class CDispatcherBase<T> : IDispatcher<T>
    {
        protected readonly ObservableSubject<T> _observable;
        protected ConcurrentQueue<T> _messageQueue;
        protected bool cancel = true;

        protected CDispatcherBase(ObservableSubject<T> observableSubject)
        {
            _observable = observableSubject;
            _messageQueue = new ConcurrentQueue<T>();
        }

        protected void Notify(T value)
        {
            _observable.Notify(value);
        }

        protected void Notify(Exception ex)
        {
            _observable.Notify(ex);
        }

        public abstract void Dispatch(TimeSpan timeInterval);

        public void Dispose()
        {
            _observable.Dispose();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observable.Subscribe(observer);
        }

        public void Push(T state)
        {
            _messageQueue.Enqueue(state);
        }

        public void StopDispatch()
        {
            cancel = false;
        }

        protected void DoDispatch(TimeSpan timeInterval)
        {
            while (true)
            {
                if (!cancel) break;
                T message;
                while (_messageQueue.TryDequeue(out message))
                {
                    Notify(message);
                }
                Thread.Sleep(timeInterval);
            }
        }
    }
}
