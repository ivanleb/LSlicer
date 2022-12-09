using Prism.Commands;
using System;

namespace LSlicer.Helpers
{
    public class ShowExceptionDelegateCommand<T> : DelegateCommand<T>
    {
        private int _repeatCount = 1;
        public int RepeatCount
        {
            get => _repeatCount;
            set => _repeatCount = value >= 0 ? value : 0;
        }
        public ShowExceptionDelegateCommand(Action<T> executeMethod) : base(executeMethod)
        {
        }

        public ShowExceptionDelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }

        protected override void Execute(object parameter)
        {
            using (RepeatableExecutor<object>
                   .New(base.Execute)
                   .SetExceptionAction(ActionHelper.ShowError)
                   .SetFinalAction(ActionHelper.ShowExcuseMessage)
                   .SetMaxRetrays(RepeatCount)
                   .SetRepeatSwitcher(ActionHelper.RepeatOffer)
                   .Build().Execute(parameter)) { }
        }
    }

    public class ShowExceptionDelegateCommand : DelegateCommand
    {
        private int _repeatCount = 1;
        public int RepeatCount 
        { 
            get => _repeatCount;
            set => _repeatCount = value >= 0 ? value : 0;
        }
        public ShowExceptionDelegateCommand(Action executeMethod) : base(executeMethod)
        {
        }

        public ShowExceptionDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }

        protected override void Execute(object parameter)
        {
            using (RepeatableExecutor<object>
                   .New(base.Execute)
                   .SetExceptionAction(ActionHelper.ShowError)
                   .SetFinalAction(ActionHelper.ShowExcuseMessage)
                   .SetMaxRetrays(RepeatCount)
                   .SetRepeatSwitcher(ActionHelper.RepeatOffer)
                   .Build().Execute(parameter)) { }
        }
    }
}
