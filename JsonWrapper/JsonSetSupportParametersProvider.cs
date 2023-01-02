using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace JsonWrapper
{
    public class JsonSetSupportParametersProvider : ISetParametersProvider<ISupportParameters>, IParametersProvider<ISupportParameters>
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public ISupportParameters GetParameters(FileInfo fileInfo)
        {
            if (!fileInfo.Exists) throw new FileNotFoundException(fileInfo.FullName);
            using (StreamReader sr = new StreamReader(fileInfo.FullName))
            using (JsonReader reader = new JsonTextReader(sr))
                return _serializer.Deserialize<SupportParameters>(reader);
        }

        public bool SetParameters(ISupportParameters parameters, FileInfo fileInfo)
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

            var jObject = JObject.FromObject(parameters);
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
