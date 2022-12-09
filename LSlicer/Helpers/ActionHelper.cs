using LSlicer.BL.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LSlicer.Helpers
{
    public static class ActionHelper
    {
        public static void ShowError(Exception e)
        {
            MessageBox.Show(e.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowExcuseMessage()
        {
            MessageBox.Show("Operation has been failed", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public static bool RepeatOffer()
        {
            return MessageBoxResult.Yes == MessageBox.Show("Repeat?", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        public static void ShowSuccessMessage(String message)
        {
            MessageBox.Show(message, "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Repeat<T>(Action<T> action, T arg)
        {
            string message = "Repeat?";
            var answer = MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer == MessageBoxResult.Yes)
            {
                try
                {
                    action?.Invoke(arg);
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }

        }
        public static void SafeExecutionWithQuestion(string question, Action action)
        {
            var answer = MessageBox.Show(question, "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer.ToString() == "Yes")
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
        }

        public static void SafeExecution<T>(Action<T> action, T arg)
        {
            try
            {
                action?.Invoke(arg);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void SafeExecution<T, U>(Action<T, U> action, T arg1, U arg2)
        {
            try
            {
                action?.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void SafeExecutionWithLogging<T>(ILoggerService logger, Action<T> action, T arg)
        {
            try
            {
                action?.Invoke(arg);
            }
            catch (Exception e)
            {
                ShowError(e);
                logger.Error($"{action.GetMethodInfo().Name} arg:\"{arg}\"", e);
            }
        }

        public static void SafeExecutionWithLogging<T, U>(ILoggerService logger, Action<T, U> action, T arg1, U arg2)
        {
            try
            {
                action?.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                ShowError(e);
                logger.Error($"{action.GetMethodInfo().Name} args:\"{arg1}\", \"{arg2}\"", e);
            }
        }
    }
}
