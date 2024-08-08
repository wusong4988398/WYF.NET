using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WYF
{
    /// <summary>
    /// JSONArray转换
    /// </summary>
    public class JSONArrayConverter : JsonConverter
    {
        // Methods
        public override bool CanConvert(Type objectType)
        {
            return typeof(JSONArray).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            JSONArray array = new JSONArray();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    array.Add(serializer.Deserialize<JSONObject>(reader));
                }
                else
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        array.Add(serializer.Deserialize<JSONArray>(reader));
                        continue;
                    }
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        return array;
                    }
                    array.Add(reader.Value);
                }
            }
            return array;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
