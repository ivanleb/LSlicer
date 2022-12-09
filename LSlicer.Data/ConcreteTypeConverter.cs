using Newtonsoft.Json;
using System;

namespace LSlicer.Data
{
    public class ConcreteTypeConverter<TConcreteType> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => serializer.Deserialize<TConcreteType>(reader);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);
    }
}
