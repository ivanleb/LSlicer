using System;

namespace LSlicer.Helpers
{
    public class RepeatableExecutor<T> : IDisposable
    {
        Action<Exception> _exceptionAction;
        Action<T> _action;
        Action _finalAction;
        Func<bool> _repeatSwitcher = () => true;
        bool _isThrow = false;
        int _repeatCounter = 0;
        int _maxRepeatCount = 1;

        public RepeatableExecutor() { }

        public RepeatableExecutor(int attempsCount, Action<T> action, Action<Exception> exceptionAction, Action finalAction)
            : this(action, exceptionAction, finalAction)
        {
            _maxRepeatCount = attempsCount;
        }


        public RepeatableExecutor(int attempsCount, Action<T> action, Action<Exception> exceptionAction, Action finalAction, Func<bool> repeatSwitcher)
            : this(attempsCount, action, exceptionAction, finalAction)
        {
            _repeatSwitcher = repeatSwitcher;
        }

        public RepeatableExecutor(Action<T> action, Action<Exception> exceptionAction, Action finalAction)
        {
            _exceptionAction = exceptionAction;
            _action = action;
            _finalAction = finalAction;
        }

        public IDisposable Execute(T arg) 
        {
            do
            {
                try
                {
                    _repeatCounter++;
                    _action?.Invoke(arg);
                    _isThrow = false;
                }
                catch (Exception e)
                {
                    _exceptionAction?.Invoke(e);
                    _isThrow = _repeatSwitcher.Invoke();
                }
            } while (_isThrow && _repeatCounter <= _maxRepeatCount);

            return this;
        }

        public void Dispose()
        {
            if(_isThrow)
                _finalAction?.Invoke();
        }

        #region Builder
        public class RepeatableExecutorBuilder<TParam> 
        {
            private Action<Exception> _exceptionAction;
            private Action _finalAction;
            private int _maxRepeatCount = 1;
            private Action<TParam> _action;
            private Func<bool> _repeatSwitcher = () => true;

            public RepeatableExecutorBuilder(Action<TParam> action)
            {
                _action = action;
            }

            public RepeatableExecutorBuilder<TParam> SetExceptionAction(Action<Exception> exceptionAction) 
            {
                _exceptionAction = exceptionAction;
                return this;
            }

            public RepeatableExecutorBuilder<TParam> SetFinalAction(Action finalAction)
            {
                _finalAction = finalAction;
                return this;
            }

            public RepeatableExecutorBuilder<TParam> SetRepeatSwitcher(Func<bool> switcher)
            {
                _repeatSwitcher = switcher;
                return this;
            }

            public RepeatableExecutorBuilder<TParam> SetMaxRetrays(int count)
            {
                _maxRepeatCount = count;
                return this;
            }

            public RepeatableExecutor<TParam> Build() 
            {
                return new RepeatableExecutor<TParam>(_maxRepeatCount, _action, _exceptionAction, _finalAction, _repeatSwitcher);
            }
        }

        public static RepeatableExecutorBuilder<T> New(Action<T> action) 
        {
            return new RepeatableExecutorBuilder<T>(action);
        }
        #endregion Builder
    }

}
