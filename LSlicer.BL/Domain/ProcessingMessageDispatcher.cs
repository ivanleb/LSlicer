using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSlicer.BL.Domain
{
    public class ProcessingMessageDispatcher<T> : CDispatcherBase<T>
    {
        public static ProcessingMessageDispatcher<T> Create()
        {
            return new ProcessingMessageDispatcher<T>(new ObservableSubject<T>());
        }
        public ProcessingMessageDispatcher(ObservableSubject<T> observableSubject) : base(observableSubject)
        {

        }


        public override void Dispatch(TimeSpan timeInterval)
        {
            cancel = true;
            Thread dispatchThread = new Thread(() => DoDispatch(timeInterval));
            dispatchThread.Start();
        }
    }
}
