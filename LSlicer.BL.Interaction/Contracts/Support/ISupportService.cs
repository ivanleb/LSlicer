using LSlicer.Data.Interaction;
using System.Collections.Generic;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface ISupportService
    {
        void MakeSupports(IList<IPart> parts);
    }
}