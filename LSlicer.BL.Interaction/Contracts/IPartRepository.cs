using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction
{
    public interface IPartRepository : IRepository<IPart>
    {
        IEnumerable<IPart> GetAll();
        IPart GetById(int id);
        bool Attach(IPart part, int attachId);
        int GetNextId();
        void RemoveAt(int id);
        int Copy(int id);
    }
}
