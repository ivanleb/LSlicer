using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public class AutoDisposeStack : IEnumerable, IDisposable 
    {
        private Stack<IDisposable> _disposeStack = new Stack<IDisposable>();
        private bool _allowNullObjects = false;
        private bool _throwItemDisposeException = false;

        public int Count => _disposeStack.Count;

        public T Add<T>(T disposableObject) where T : IDisposable
        {
            if (disposableObject == null && !_allowNullObjects)
                throw new ArgumentNullException("Disposable object can not be null");
            _disposeStack.Push(disposableObject);
            return disposableObject;
        }

        public void AddRange(IEnumerable<IDisposable> disposableObjects)
        {
            foreach (var item in disposableObjects)
            {
                if (item == null && !_allowNullObjects)
                    throw new ArgumentNullException("Disposable object can not be null");
                _disposeStack.Push(item);
            }
        }

        public void Dispose()
        {
            List<Exception> exceptions = new List<Exception>();
            while (_disposeStack.Count > 0)
            {
                try
                {
                    IDisposable item = _disposeStack.Pop();
                    if (item == null && _allowNullObjects) continue;

                    Task task = item as Task;
                    if (task != null && !task.IsCompleted)
                    {
                        exceptions.Add(new InvalidOperationException($"Task {task.Id} has been {task.Status}"));
                        continue;
                    }

                    item.Dispose();
                }
                catch (Exception e)
                {
                    if (_throwItemDisposeException) throw;
                    exceptions.Add(e);
                    try
                    {
                        if (exceptions.Count > 0) throw new AggregateException(exceptions);
                    }
                    catch (Exception)
                    {
                        throw; //log
                    }
                }
            }
        }

        public void Remove(IDisposable disposableObject)
        {
            var list = _disposeStack.ToList();
            list.Reverse();
            list.Remove(disposableObject);
            _disposeStack.Clear();
            AddRange(list);
        }

        public void SafeClear()
        {
            try
            {
                _disposeStack.Clear();
            }
            catch (Exception)
            {
                if (_throwItemDisposeException) throw;
            }
        }

        public IDisposable[] ToArray()
        {
            return _disposeStack.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return _disposeStack.GetEnumerator();
        }
    }
}
