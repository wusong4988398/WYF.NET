using System.Collections.Concurrent;
using System;

namespace WYF.Algo
{
    public abstract class Hint : ConcurrentDictionary<string, object>
    {
        // 构造函数
        protected Hint() : base()
        {
        }

        // 检查键是否存在
        protected bool HasKey(string key)
        {
            return ContainsKey(key);
        }

        // 获取整数值
        public int GetInt(string key, int defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;

            var value = this[key];
            if (value == null)
                return defaultValue;

            if (value is int intValue)
                return intValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }

        // 获取布尔值
        public bool GetBoolean(string key, bool defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;

            var value = this[key];
            if (value == null)
                return defaultValue;

            if (value is bool boolValue)
                return boolValue;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }

        // 获取字符串值
        public string GetString(string key, string defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;

            var value = this[key];
            if (value == null)
                return defaultValue;

            return value.ToString();
        }
    }
}