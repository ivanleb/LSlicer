using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicing.Data.Interaction
{
    public interface ICopyable<T> where T : IIdentifier
    {
        T Copy(int newId);
    }
}
