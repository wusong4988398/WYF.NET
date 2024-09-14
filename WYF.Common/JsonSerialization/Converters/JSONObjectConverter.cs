using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WYF
{
    public class JSONObjectConverter : JsonConverter
    {
        /// <summary>
        /// 是否开启自定义反序列化，默认值为true时，反序列化时会走ReadJson方法，值为false时，不走ReadJson方法，而是默认的反序列化
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(JSONObject).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            JSONObject obj2 = new JSONObject();
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    return obj2;
                }
                string propName = reader.Value.ToString();
                reader.Read();
                if (reader.TokenType == JsonToken.StartObject)
                {
                    JSONObject obj3 = serializer.Deserialize<JSONObject>(reader);
                    obj2.Put(propName, obj3);
                }
                else
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        JSONArray array = serializer.Deserialize<JSONArray>(reader);
                        obj2.Put(propName, array);
                        continue;
                    }
                    obj2.Put(propName, reader.Value);
                }
            }
            return obj2;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        /// <summary>
        ///  //是否开启自定义序列化，默认值为true时，序列化时会走WriteJson方法，值为false时，不走WriteJson方法，而是默认的序列化
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
