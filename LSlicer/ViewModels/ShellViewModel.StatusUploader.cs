using System;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel
    {
        private class StatusUploader : IObserver<string>
        {
            private Action<string> _statusChanger;

            public StatusUploader(Action<string> statusChanger)
            {
                _statusChanger = statusChanger;
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(string value)
            {
                _statusChanger(value);
            }

            public static string ProgressFilter(string status) => status.ToLower().Contains("progress") ? status : String.Empty;
            public static string ExceptProgressFilter(string status) => (!status.ToLower().Contains("progress")) ? status : String.Empty;
        }

    }
}