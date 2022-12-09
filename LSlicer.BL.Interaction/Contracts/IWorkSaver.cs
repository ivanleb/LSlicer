using LSlicer.Data.Interaction;
using System.Collections.Generic;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface IWorkSaver
    {
        string Save(string name, IEnumerable<IPart> parts);
    }
}
