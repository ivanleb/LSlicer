using LSlicer.BL.Interaction;
using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LSlicer.BL.Domain
{
    public abstract class EngineInvokerBase<T> : IEngineInvoker<T>
    {
        private readonly ILoggerService _loggerService;
        private IList<IObserver<T>> _observers = new List<IObserver<T>>();

        public EngineInvokerBase(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public void Run(IEngineTask engineTask, IEngineResultAwaiter engineResultAwaiter, CancellationToken cancellationToken)
        {

            CmdLine cmd = InterpretateSpec(engineTask);
            ProcessRunner runner = new ProcessRunner(_loggerService);

            using (var disposeStack = new AutoDisposeStack())
            {
                foreach (var observer in _observers)
                    disposeStack.Add(runner.Subscribe((IObserver<string>)observer));

                runner.Run(cmd,
                    (t) =>
                    {
                        if (t.Status != TaskStatus.Faulted)
                            engineResultAwaiter.GetEngineTaskAwaiter(engineTask)?.Invoke();
                    },
                    cancellationToken);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber<T>(_observers, observer);
        }

        protected abstract CmdLine InterpretateSpec(IEngineTask engineTask);
    }
}
