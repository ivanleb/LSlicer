using LSlicer.BL.Interaction;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LSlicer.BL.Domain
{
    public class CloseAppHandler : ICloseApplicationHandler
    {
        private readonly Dictionary<string, Action> _delegates = new Dictionary<string, Action>();
        private readonly object _locker = new object();

        private readonly ILoggerService _logger;

        public CloseAppHandler(ILoggerService logger)
        {
            _logger = logger;
        }

        public void Add(string actionName, Action action)
        {
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException($"{nameof(actionName)} is empty");

            if (action == null)
                throw new ArgumentNullException($"{actionName} is null");

            lock(_locker)
                _delegates.Add(actionName, action);
        }

        public void Handle()
        {
            lock (_locker)
            {
                foreach (Action closeProgramDelegate in _delegates.Values)
                {
                    try
                    {
                        closeProgramDelegate?.Invoke();
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"[{nameof(CloseAppHandler)}] Method: \"{closeProgramDelegate.GetMethodInfo().Name}\"", e);
                    }
                }
            }
        }

        public bool Remove(string actionName)
        {
            lock (_locker)
                return _delegates.Remove(actionName);
        }
    }
}
