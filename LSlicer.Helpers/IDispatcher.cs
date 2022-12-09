using System;

namespace LSlicer.Helpers
{
    public interface IDispatcher<T> : IObservable<T>
    {
        void Dispatch(TimeSpan timeInterval);
        void StopDispatch();
        void Push(T state);
    }
}
