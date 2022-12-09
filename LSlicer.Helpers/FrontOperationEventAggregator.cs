using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{

    public class FrontOperationEventAggregator : IFrontOperationEventAggregator
    {
        private readonly Dictionary<Type, FrontOperationEvent> _events = new Dictionary<Type, FrontOperationEvent>();
        public FrontOperationEvent GetEvent<T>() where T : IFrontOperationEventParameter
        {
            return _events[typeof(T)];
        }

        public void AddEvent<T>(Action<T> action) where T : IFrontOperationEventParameter
        {
            var @event = new FrontOperationEvent();
            @event.Subscribe(action);
            _events[typeof(T)] = @event;
        }
    }


    public class FrontOperationEvent
    {
        private Action _action;
        private Type _parameterType; 
        private object _parameter;
        public void Subscribe<T>(Action<T> action) where T : IFrontOperationEventParameter
        {
            _parameterType = typeof(T);
            _action = () => action.Invoke((T)_parameter); 
        }
        public void Unsubscribe<T>(Action<T> action) where T : IFrontOperationEventParameter
        { 
            if (_action != null && _parameterType == typeof(T)) 
                _action = null; 
        }
        public void Publish<T>(T parameter) where T : IFrontOperationEventParameter
        {
            if (_parameterType != typeof(T))
                return;
            _parameter = parameter;
            _action?.Invoke();
        }
    }
}
