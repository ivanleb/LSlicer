using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public static class AppFileHelper
    {
        public static void CopyIfDoesntExist(string defaultParametersPath, string parametersFile)
        {
            if (!File.Exists(parametersFile))
            {
                if (File.Exists(defaultParametersPath))
                {
                    File.Copy(defaultParametersPath, parametersFile);
                }
                else throw new FileNotFoundException(defaultParametersPath);
            }
        }
    }
}
