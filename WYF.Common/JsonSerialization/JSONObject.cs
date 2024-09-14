using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    [Serializable]
    public class JSONObject : Dictionary<string, object>
    {

        private static HashSet<JsonConverter> converters = new HashSet<JsonConverter>();
        static JSONObject()
        {
            //RegisterConvertor(new ContextConverter());
            RegisterConvertor(new JSONArrayConverter());
            RegisterConvertor(new JSONObjectConverter());
        }

        public static JSONObject Parse(string json)
        {
            return (JSONObject)JsonConvert.DeserializeObject(json, typeof(JSONObject), converters.ToArray<JsonConverter>());
        }
        public object Get(string propName)
        {
            object obj2 = null;
            base.TryGetValue(propName, out obj2);
            return obj2;
        }
        public bool GetBool(string propName)
        {
            object obj2 = false;
            base.TryGetValue(propName, out obj2);
            return Convert.ToBoolean(obj2);
        }
        private static T GetDefaultValue<T>()
        {
            Type type = typeof(T);
            object obj2 = type.IsValueType ? ((object)default(T)) : ((type == typeof(string)) ? ((object)string.Empty) : type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
            return (T)obj2;
        }
        public int GetInt(string propName)
        {
            object obj2 = null;
            if (base.TryGetValue(propName, out obj2))
            {
                int value = 0;

                if (Int32.TryParse(obj2.ToString(), out value))
                {
                    return value;
                }

                //return Convert.ToInt32(obj2);
            }
            return 0;
        }
        public static void RegisterConvertor(JsonConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            converters.Add(converter);
        }

        public JSONObject GetJSONObject(string propName)
        {
            object obj2 = null;
            base.TryGetValue(propName, out obj2);
            return (obj2 as JSONObject);
        }

        public long GetLong(string propName)
        {
            return (long)base[propName];
        }

        public string GetString(string propName)
        {
            object obj2 = null;
            base.TryGetValue(propName, out obj2);
            return (string)obj2;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object obj2;
            if (!base.TryGetValue(key, out obj2))
            {
                return defaultValue;
            }
            if (((obj2 != null) && (obj2.GetType().Name == typeof(long).Name)) && (typeof(T) == typeof(int)))
            {
                obj2 = int.Parse(obj2.ToString());
            }

            return (T)Convert.ChangeType(obj2, typeof(T));
            //return DBReaderUtils.ConvertTo<T>(obj2, null);
        }

        public void Put(string propName, object value)
        {
            base[propName] = value;
        }
        public string ToJSONString()
        {
            return JsonConvert.SerializeObject(this, converters.ToArray<JsonConverter>());
        }
        public override string ToString()
        {
            return this.ToJSONString();
        }
        public bool TryGetValue(string key, out object v, bool removeItem)
        {
            if (!base.TryGetValue(key, out v))
            {
                return false;
            }
            if (removeItem)
            {
                base.Remove(key);
            }
            return true;
        }


    }
}
