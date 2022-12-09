using System;
using System.Threading;

namespace LSlicer.BL.Interaction
{
    public interface IEngineInvoker<T> : IObservable<T>
    {
        void Run(IEngineTask engineTask, IEngineResultAwaiter engineResultAwaiter, CancellationToken cancellationToken);
    }
}
