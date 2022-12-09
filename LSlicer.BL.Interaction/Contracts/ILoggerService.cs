using System;

namespace LSlicer.BL.Interaction
{
    public enum LogType 
    {
        Info,
        Error,
        Debug,
        Fatal
    }

    public interface ILoggerService
    {
        void TypedLog(LogType logType, object logInfo);
        void Debug(string message, Exception ex);
        void Info(string message);
        void Error(string message, Exception ex);
        void Fatal(Exception ex);
        void RunWithExceptionLogging(Action actionToRun, bool isSilent = false);
        T RunWithExceptionLogging<T>(Func<T> functionToRun, bool isSilent = false);
    }
}