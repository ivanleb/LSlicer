using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.IO;

namespace LSlicer.BL.Interaction
{
    public interface IPartService
    {
        IList<IPart> Parts { get; }
        void ApplyTransform(IPartTransform partTransform, int[] partsForTransform);
        int Copy(int id);
        int LoadPart(FileInfo path);
        void UnloadPart(int id);
        void SaveWorkState(SavingWorkStateSpec spec);
        void LoadWorkState(LoadingWorkStateSpec spec);
        void SliceParts(DirectoryInfo path);
        void LoadSlicingInfos(ISlicingInfo[] infos);
        void MakeSupports(DirectoryInfo path);
        IOperationManager GetOperationManager();
        IEnumerable<ISlicingInfo> GetSlicingInfo(int partId);
        void CancelActiveOperations();
    }
}
