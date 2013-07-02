using System;
using Newtonsoft.Json;

namespace PerpetuumSoft.Knockout
{
    public class JsonDateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dt = ((DateTime)(value));
            if (dt == default(DateTime))
                writer.WriteValue("");
            else
                writer.WriteValue(dt.ToString("dd/MM/yyyy HH:mm"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
    }

    public class JsonDateConverter : JsonDateTimeConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dt = ((DateTime)(value));
            if (dt == default(DateTime))
                writer.WriteValue("");
            else
                writer.WriteValue(dt.ToString("dd/MM/yyyy"));
        }
    }
}
