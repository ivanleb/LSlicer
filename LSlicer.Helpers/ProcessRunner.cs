using LSlicer.BL.Interaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public class ProcessRunner : IObservable<string>
    {
        private readonly ILoggerService _logger;
        private IList<IObserver<string>> _observers;

        public ProcessRunner(ILoggerService logger)
        {
            _logger = logger;
            _observers = new List<IObserver<string>>();
        }

        public void Run(CmdLine cmdLine, Action<Task> gettingResult, CancellationToken cancellationToken)
        {
            _logger.Info($"[{nameof(ProcessRunner)}] Run executable \"{cmdLine.GetProcessPath()}\" with parameters \"{cmdLine.GetArgs()}\" ");
            Task.Run(() =>
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = cmdLine.GetProcessPath(),
                            Arguments = cmdLine.GetArgs(),
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();

                    _logger.RunWithExceptionLogging(() =>
                    {
                        while (!process.StandardOutput.EndOfStream)
                        {
                            if (cancellationToken.IsCancellationRequested) 
                            {
                                process.Kill();
                            }

                            var line = process.StandardOutput.ReadLine();

                            foreach (var observer in _observers)
                                observer.OnNext(line);
                        }
                    });
                    process.WaitForExit();

                }
                catch (Exception e)
                {
                    _logger.Error("[ProcessRunner] Engine error: ", e);
                    throw;
                }
                finally
                {
                    EndProcessing();
                    _logger.Info($"[ProcessRunner] Executable  \"{cmdLine.GetProcessPath()}\"  working end.");
                }
            })
                .ContinueWith(gettingResult);
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber<string>(_observers, observer);
        }

        public void EndProcessing()
        {
            foreach (var observer in _observers)
                if (_observers.Contains(observer))
                    observer.OnCompleted();

            _observers.Clear();
        }
    }
}
