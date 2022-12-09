using LaserAprSlicer.BL.Interaction.Contracts;
using System;

namespace LaserAprSlicer.BL.Domain
{
    public class SupportEngineInvoker<T> : IEngineInvoker<T>
    {
        public void Run(IEngineTask engineTask)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }
    }
}
