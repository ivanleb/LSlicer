using EngineHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradientSpaceSliceEngine
{
    public static class gsJobSpecChecker
    {
        public static bool Check(JobSpecification jobSpecification) 
        {
            bool isOk = true;
            if (jobSpecification.JobType.HasValue)
                Logger.Log.Info($"Job is {jobSpecification.JobType.Value.ToString()}");
            else
            {
                Logger.Log.Info($"Job is empty");
                isOk = false;
            }

            if (jobSpecification.ModelFileInfo.All(f => f.Exists))
            {
                foreach (var ModelFileInfo in jobSpecification.ModelFileInfo)
                    Logger.Log.Info($"File {ModelFileInfo.FullName} is exist");
            }
            else if (jobSpecification.ModelFileInfo == null)
            {
                Logger.Log.Info($"Model file path is empty");
                isOk = false;
            }
            else
            {
                Logger.Log.Info($"Model file path doesnt exist");
                isOk = false;
            }

            if (jobSpecification.ParametersFileInfo != null && jobSpecification.ParametersFileInfo.Exists)
                Logger.Log.Info($"Parameters file {jobSpecification.ParametersFileInfo.FullName} is exist");
            else if (jobSpecification.ParametersFileInfo == null)
            {
                Logger.Log.Info($"Parameters file path is empty");
                isOk = false;
            }
            else
            {
                Logger.Log.Info($"Parameters file path doesnt exist");
                isOk = false;
            }

            return isOk;
        }
    }
}
