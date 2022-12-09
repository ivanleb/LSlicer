using LSlicer.Data.Interaction;
using System.Collections.Generic;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface ISavedLoader
    {
        void LoadSavedParts(IEnumerable<IPartDataForSave> parts);
    }
}
