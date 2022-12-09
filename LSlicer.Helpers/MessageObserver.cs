using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public class MessageObserver<T> : IObserver<T>
    {
        DispatcherAggregator<T> _dispatcher;
        public MessageObserver(DispatcherAggregator<T> dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void OnCompleted()
        {
            _dispatcher.StopDispatch();
        }

        public void OnError(Exception error)
        {
            _dispatcher.StopDispatch();
        }

        public void OnNext(T value)
        {
            _dispatcher.Push(value);
        }
    }
}
