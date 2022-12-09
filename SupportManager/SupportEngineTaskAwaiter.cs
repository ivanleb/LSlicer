using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportManager
{
    public class SupportEngineTaskAwaiter : IEngineResultAwaiter
    {
        private readonly ILoggerService _loggerService;
        private readonly IPostProcessor<IPart> _postProcessor;

        public SupportEngineTaskAwaiter(IPostProcessor<IPart> postProcessor, ILoggerService loggerService)
        {
            _postProcessor = postProcessor;
            _loggerService = loggerService;
        }

        public Action GetEngineTaskAwaiter(IEngineTask awaitingTask)
        {
            //ISupport[] supports = awaitingTask.PartSpec.Select(ps => ps.Support)
            //    .Where(f => File.Exists(f.PartSpec.MeshFilePath)).ToArray();

            return () =>
            {
                if (!File.Exists(awaitingTask.JobResult.FullName)) return;

                using (var reader = new StreamReader(awaitingTask.JobResult.FullName))
                {
                    string json = reader.ReadToEnd();
                    var parameters = JsonConvert.DeserializeObject<List<SupportInfo>>(json)
                                                .Cast<ISupportInfo>().ToList();

                    IPart[] supports = new IPart[parameters.Count];
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        IPartSpec partSpec = new PartSpec(0, parameters[i].SupportFilePath);
                        int link = awaitingTask.PartSpec.Where(s => s.FilePath == parameters[i].MeshFilePath)
                                                         .Select(ps => ps.PartId).FirstOrDefault();

                        supports[i] = new Support(partSpec, link);
                    }

                    _loggerService.Info($"Post process supports");
                    _postProcessor.HandleResult(supports);
                }
            };
        }
    }
}
