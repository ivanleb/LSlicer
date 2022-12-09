using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using System;

namespace LSlicer.BL.Interaction
{
    public interface IWorkStateManager
    {
        void AddChangedPartsIntoManager(params IPart[] parts);
        void Save(SavingWorkStateSpec spec);
        IPartDataForSave[] LoadSavedData(LoadingWorkStateSpec spec);
    }
}
