using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlicingManager
{
    public class SlicingEngineTaskAwaiter : IEngineResultAwaiter
    {
        private readonly ILoggerService _loggerService;
        IPostProcessor<ISlicingInfo> _slicingPostProcessor;

        public SlicingEngineTaskAwaiter(IPostProcessor<ISlicingInfo> slicingPostProcessor,  ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _slicingPostProcessor = slicingPostProcessor;
        }

        public Action GetEngineTaskAwaiter(IEngineTask awaitingTask)
        {
            return () => 
            {
                if (File.Exists(awaitingTask.JobResult.FullName))
                {
                    using (var reader = new StreamReader(awaitingTask.JobResult.FullName))
                    {
                        string json = reader.ReadToEnd();
                        var parameters = JsonConvert.DeserializeObject<List<SlicingInfo>>(json).Cast<ISlicingInfo>().ToList();
                        
                        List<ISlicingInfo> infos = new List<ISlicingInfo>();

                        foreach (var param in parameters)
                            foreach (var part in awaitingTask.PartSpec)
                                if (Path.GetFileNameWithoutExtension(part.ResultFilePath) == Path.GetFileNameWithoutExtension(param.FilePath))
                                { 
                                    infos.Add(param); 
                                }

                        _slicingPostProcessor.HandleResult(infos.ToArray());

                        _loggerService.Info($"Get slicing info from file \"{awaitingTask.JobResult.FullName}\".");
                    }
                }
                else
                    _loggerService.Info($"No slicing info file \"{awaitingTask.JobResult.FullName}\".");
            };
        }
    }
}
