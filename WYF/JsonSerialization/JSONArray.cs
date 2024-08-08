using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class JSONArray : List<object>
    {
        // Fields
        private static HashSet<JsonConverter> converters = new HashSet<JsonConverter>();

        // Methods
        static JSONArray()
        {
            //RegisterConvertor(new ContextConverter());
            RegisterConvertor(new JSONArrayConverter());
            RegisterConvertor(new JSONObjectConverter());
        }

        public JSONArray()
        {
        }

        public JSONArray(string parameters)
        {
            JSONArray jsonarray = JsonConvert.DeserializeObject<JSONArray>(parameters);

            if (jsonarray != null)
            {
                base.AddRange(jsonarray);
            }
        }

        public T ConvertToObject<T>()
        {
            return (T)new JsonSerializer { Converters = { new JSONArrayConverter(), new JSONObjectConverter() } }.Deserialize(new StringReader(this.ToJSONString()), typeof(T));
        }

        public JSONArray DeepClone()
        {
            return (JSONArray)JsonConvert.DeserializeObject(this.ToJSONString(), typeof(JSONArray), converters.ToArray<JsonConverter>());
        }

        public string GetJsonString(int i)
        {
            object obj2 = base[i];
            return JsonConvert.SerializeObject(obj2);
        }

        public string GetString(int i)
        {
            return (string)base[i];
        }

        public static JSONArray Parse(string json)
        {
            return new JSONArray(json);
        }

        public static void RegisterConvertor(JsonConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            converters.Add(converter);
        }

        public string ToJSONString()
        {
            return JsonConvert.SerializeObject(this, converters.ToArray<JsonConverter>());
        }

        public override string ToString()
        {
            return this.ToJSONString();
        }
    }
}
