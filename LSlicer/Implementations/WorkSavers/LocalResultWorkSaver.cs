using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Data.Operations;
using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSlicer.Implementations
{
    public class LocalResultWorkSaver : IWorkSaver
    {
        private readonly ILoggerService _logger;
        private readonly IAppSettings _appSettings;
        private readonly IOperationStack _operationStack;

        public LocalResultWorkSaver(ILoggerService logger, IAppSettings appSettings, IOperationStack operationStack)
        {
            _logger = logger;
            _appSettings = appSettings;
            _operationStack = operationStack;
        }

        public string Save(string name, IEnumerable<IPart> parts)
        {
            try
            {
                string path = DirectorySelector.Select(_appSettings.WorkingDirectory);
                if (parts.Count() == 0)
                    throw new InvalidOperationException($"[{nameof(LocalResultWorkSaver)}] Collection is empty {nameof(parts)}");

                DirectoryInfo directory = new DirectoryInfo(path);
                if (!directory.Exists) 
                    throw new DirectoryNotFoundException(path);

                DirectoryInfo resultDirectory = new DirectoryInfo(Path.Combine(path, name));
                resultDirectory.Create();
                _logger.Info($"[{nameof(LocalResultWorkSaver)}] Directory {resultDirectory.FullName} was been created.");
                foreach (var part in parts)
                {
                    var slicingInfos = _operationStack.GetOperationsByPart(part.Id).GetOperationResultInfoByType<ISlicingInfo>();
                    //var slicingInfos = part.Operations.GetOperationResultInfoByType<ISlicingInfo>();
                    foreach (var slicing in slicingInfos)
                    {
                        FileInfo slFile = new FileInfo(slicing.FilePath);
                        if (slFile.Exists)
                        {
                            var destFilePath = Path.Combine(resultDirectory.FullName, Path.GetFileName(slicing.FilePath));
                            slFile.CopyTo(destFilePath);
                            _logger.Info($"[{nameof(LocalResultWorkSaver)}] Copy {slFile.Name} to {destFilePath}.");
                        }
                    }
                }
                return resultDirectory.FullName;
            }
            catch (Exception e)
            {
                _logger.Error($"[{nameof(LocalResultWorkSaver)}]", e);
                throw;
            }
        }
    }
}
