using LSlicer.Data.Interaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonWrapper
{
    public static class JsonSlicingResultProvider
    {
        public static void WriteInfo(ISlicingInfo[] slicingInfos, FileInfo file) 
        {
            string json = JsonConvert.SerializeObject(slicingInfos, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(file.FullName, append: false))
            {
                sw.Write(json);
            }
        }
    }
}
