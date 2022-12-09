using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace LSlicer.BL.Domain
{
    public class PartSerializer : IPartSerializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings 
        { 
            TypeNameHandling = TypeNameHandling.All, 
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        };
        public PartDataForSave[] Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<PartDataForSave[]>(data, _settings);
        }

        public string Serialize(PartDataForSave[] dataForSave)
        {
            return JsonConvert.SerializeObject(dataForSave, Formatting.Indented, _settings);
        }
    }
}
