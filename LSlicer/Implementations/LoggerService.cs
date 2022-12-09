using LSlicer.BL.Interaction;
using log4net;
using System;

namespace LSlicer.Implementations
{
    public class LoggerService : ILoggerService
    {
        private readonly ILog _logger;

        public LoggerService(ILog logger)
        {
            _logger = logger;
        }

        public void TypedLog(LogType logType, object logInfo) 
        {
            switch (logType)
            {
                case LogType.Info:
                    Info(logInfo.ToString());
                    break;
                case LogType.Error:
                    Error(logInfo.ToString(), logInfo as Exception);
                    break;
                case LogType.Debug:
                    Debug(logInfo.ToString(), logInfo as Exception);
                    break;
                case LogType.Fatal:
                    if (logInfo is Exception exceptionLog)
                        Fatal(exceptionLog);
                    break;
                default:
                    break;
            }
        }

        public void Debug(string message, Exception ex) => _logger.Debug(message, ex);

        public void Error(string message, Exception ex) => _logger.Error($"ERROR: {message} ", ex);

        public void Fatal(Exception ex) => _logger.Fatal("FATAL ERROR: ", ex);

        public void Info(string message) => _logger.Info(message);

        public void RunWithExceptionLogging(Action actionToRun, bool isSilent = false)
        {
            try
            {
                actionToRun();
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR: ", ex);

                if (isSilent)
                {
                    return;
                }

                throw;
            }
        }

        public T RunWithExceptionLogging<T>(Func<T> functionToRun, bool isSilent = false)
        {
            try
            {
                return functionToRun();
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR: ", ex);

                if (isSilent)
                {
                    return default(T);
                }

                throw;
            }
        }
    }
}