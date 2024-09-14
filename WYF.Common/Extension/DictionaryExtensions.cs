using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public static class DictionaryExtensions
    {
        public static object? Get(this IDictionary<string, object> dct, string propName)
        {
            if (!dct.TryGetValue(propName, out var obj2))
            {
                return null;
            }

            return obj2;
        }
        public static string GetString(this IDictionary<string, object> dct, string propName)
        {
            return !dct.TryGetValue(propName, out var obj2) ? "" : obj2.ToNullString();
        }


        /// <summary>
        /// 获取指定键的值，不存在则按指定委托添加值
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="dictionary">要操作的字典</param>
        /// <param name="key">指定键名</param>
        /// <param name="addFunc">添加值的委托</param>
        /// <returns>获取到的值</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addFunc)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return dictionary[key] = addFunc();
        }
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out var value);
            return value;
        }
    }
}
