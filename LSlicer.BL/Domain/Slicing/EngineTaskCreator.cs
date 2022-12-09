using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using System;
using System.IO;
using System.Linq;

namespace LSlicer.BL.Domain
{
    public static class EngineTaskCreator
    {
        public static IEngineTask Create(EJobType job, IPart[] parts, FileInfo enginePath, FileInfo parametersPath, FileInfo result, int numberFrom, IOperation operation)
        {
            switch (job)
            {
                case EJobType.Slice:
                    ITaskSpec[] partSpecs =
                        parts.Select(x =>
                        {
                            var info = operation.Info;

                            if (info != null && info is ISlicingInfo slicingInfo)
                                return new TaskSpec(x.PartSpec.MeshFilePath, slicingInfo.FilePath, x.Id);

                            throw new Exception("No slicing operations");
                        }).ToArray();
                    return new SliceEngineTask(partSpecs, enginePath, parametersPath, result);

                case EJobType.MakeSupports:
                    ITaskSpec[] supportPartSpecs =
                        parts.Select(x =>
                        {
                            var info = operation.Info;

                            if (info != null && info is ISupportInfo slicingInfo)
                                return new TaskSpec(x.PartSpec.MeshFilePath, slicingInfo.SupportFilePath, x.Id);

                            throw new Exception("No support operations");
                        }).ToArray();
                    return new SupportEngineTask(supportPartSpecs, enginePath, parametersPath, result, numberFrom);

                default:
                    throw new Exception("Unknown job type");
            }
        }
    }
}
