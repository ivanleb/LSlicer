using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public interface INotifier<in T>
    {
        void Notify(T observer);
        void Notify(Exception e);
    }
}
