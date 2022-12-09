using LSlicer.BL.Interaction;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LSlicer.Data.Model;

namespace JsonWrapper
{
    public class JSONSetSlicingParametersProvider : ISetParametersProvider<ISlicingParameters>, IParametersProvider<ISlicingParameters>
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public ISlicingParameters GetParameters(FileInfo fileInfo)
        {
            if (!fileInfo.Exists) throw new FileNotFoundException(fileInfo.FullName);
            using (StreamReader sr = new StreamReader(fileInfo.FullName))
            using (JsonReader reader = new JsonTextReader(sr))
                return _serializer.Deserialize<SlicingParameters>(reader);
        }

        public bool SetParameters(ISlicingParameters slicingParameters, FileInfo fileInfo)
        {
            int attemps = 0;
            int attempCount = 5;
            while (fileInfo.Exists && attemps <= attempCount)
            {
                try 
                { 
                    File.Delete(fileInfo.FullName);
                    attemps = attempCount + 1;
                }
                catch (Exception)
                {
                    if (attemps == attempCount) throw;
                    ++attemps;
                }
            }

            var jObject = JObject.FromObject(slicingParameters);
            using (StreamWriter sw = new StreamWriter(fileInfo.FullName, append: false))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                _serializer.Serialize(writer, jObject);
            }
            fileInfo.Refresh();
            return true;
        }
    }
}
