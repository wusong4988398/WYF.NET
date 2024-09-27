using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public static class DBReaderUtils
    {
        public static string GetString(this IDataReader dr, string fieldName)
        {
            return Convert.ToString(dr[fieldName]);
        }

        public static int GetInt(this IDataReader dr, string fieldName)
        {
            return Convert.ToInt32(dr[fieldName]);
        }

        public static int GetIntCompatibleNull(this IDataReader dr, string fieldName, int NullConvert)
        {
            if (dr[fieldName] == DBNull.Value)
            {
                return NullConvert;
            }
            return Convert.ToInt32(dr[fieldName]);
        }

        public static bool GetBoolean(this IDataReader dr, string fieldName)
        {
            return Convert.ToBoolean(dr[fieldName]);
        }

        public static DateTime GetDateTime(this IDataReader dr, string fieldName)
        {
            return Convert.ToDateTime(dr[fieldName]);
        }

        public static int? GetIntEx(this IDataReader dr, string fieldName)
        {
            int? result = null;
            if (dr[fieldName] != DBNull.Value)
            {
                result = new int?(Convert.ToInt32(dr[fieldName]));
            }
            return result;
        }

        public static bool? GetBooleanEx(this IDataReader dr, string fieldName)
        {
            bool? result = null;
            if (dr[fieldName] != DBNull.Value)
            {
                result = new bool?(Convert.ToBoolean(dr[fieldName]));
            }
            return result;
        }

        public static DateTime? GetDateTimeEx(this IDataReader dr, string fieldName)
        {
            DateTime? result = null;
            if (dr[fieldName] != DBNull.Value)
            {
                result = new DateTime?(Convert.ToDateTime(dr[fieldName]));
            }
            return result;
        }


        public static T GetValue<T>(this IDataRecord dr, string fieldName)
        {
            return DBReaderUtils.ConvertTo<T>(dr[fieldName], null);
        }


        public static T GetValue<T>(this IDataRecord dr, string fieldName, Func<object, T> convertFunc)
        {
            return DBReaderUtils.ConvertTo<T>(dr[fieldName], convertFunc);
        }


        public static T GetValue<T>(this IDataRecord dr, int index)
        {
            return DBReaderUtils.ConvertTo<T>(dr.GetValue(index), null);
        }

        public static T GetValue<T>(this IDataRecord dr, int index, Func<object, T> convertFunc)
        {
            return DBReaderUtils.ConvertTo<T>(dr.GetValue(index), convertFunc);
        }

        public static T ConvertTo<T>(object value, Func<object, T> convertFunc)
        {
            return DBReaderUtils.ConvertTo<T>(value, convertFunc, default(T));
        }

        public static T ConvertTo<T>(object value, Func<object, T> convertFunc, T defaultValue)
        {
            if (Convert.IsDBNull(value) || value == null)
            {
                return defaultValue;
            }
            if (value is T)
            {
                return (T)((object)value);
            }
            if (convertFunc == null)
            {
                convertFunc = DBReaderUtils.ConvertFuncHelper<T>.Default;
            }
            if (convertFunc != null)
            {
                return convertFunc(value);
            }
            throw new ArgumentException($"数据{value}不能转换成{typeof(T)}，请尝试提供转换器参数convertFunc或修正数据");
        }

        private static Dictionary<Type, object> CreateDefaultConvertFunc()
        {
            Dictionary<Type, object> dictionary = new Dictionary<Type, object>();
            DBReaderUtils.AddConvertFunc<bool>(dictionary, new Func<object, bool>(DBReaderUtils.ToBoolean));
            DBReaderUtils.AddConvertFunc<byte>(dictionary, new Func<object, byte>(Convert.ToByte));
            DBReaderUtils.AddConvertFunc<char>(dictionary, new Func<object, char>(Convert.ToChar));
            DBReaderUtils.AddConvertFunc<DateTime>(dictionary, new Func<object, DateTime>(Convert.ToDateTime));
            DBReaderUtils.AddConvertFunc<decimal>(dictionary, new Func<object, decimal>(Convert.ToDecimal));
            DBReaderUtils.AddConvertFunc<double>(dictionary, new Func<object, double>(Convert.ToDouble));
            DBReaderUtils.AddConvertFunc<short>(dictionary, new Func<object, short>(Convert.ToInt16));
            DBReaderUtils.AddConvertFunc<int>(dictionary, new Func<object, int>(Convert.ToInt32));
            DBReaderUtils.AddConvertFunc<long>(dictionary, new Func<object, long>(Convert.ToInt64));
            DBReaderUtils.AddConvertFunc<sbyte>(dictionary, new Func<object, sbyte>(Convert.ToSByte));
            DBReaderUtils.AddConvertFunc<float>(dictionary, new Func<object, float>(Convert.ToSingle));
            DBReaderUtils.AddConvertFunc<string>(dictionary, new Func<object, string>(Convert.ToString));
            DBReaderUtils.AddConvertFunc<ushort>(dictionary, new Func<object, ushort>(Convert.ToUInt16));
            DBReaderUtils.AddConvertFunc<uint>(dictionary, new Func<object, uint>(Convert.ToUInt32));
            DBReaderUtils.AddConvertFunc<ulong>(dictionary, new Func<object, ulong>(Convert.ToUInt64));
            DBReaderUtils.AddConvertFunc<Guid>(dictionary, new Func<object, Guid>(DBReaderUtils.ToGuid));
            return dictionary;
        }

        private static Guid ToGuid(object value)
        {
            byte[] array = value as byte[];
            if (array != null)
            {
                return new Guid(array);
            }
            string text = value as string;
            if (text != null)
            {
                return new Guid(text);
            }
            throw new InvalidCastException($"无法将{value}转换为GUID类型");
        }

        private static bool ToBoolean(object value)
        {
            if (value is string)
            {
                string a = (string)value;
                if (a == string.Empty || a == "0")
                {
                    return false;
                }
                if (a == "1" || a == "-1")
                {
                    return true;
                }
            }
            return Convert.ToBoolean(value);
        }

        private static void AddConvertFunc<T>(Dictionary<Type, object> funcs, Func<object, T> func)
        {
            funcs.Add(typeof(T), func);
        }

        private static Func<object, T> GetDefaultConvertFunc<T>()
        {
            Dictionary<Type, object> dictionary = DBReaderUtils.CreateDefaultConvertFunc();
            object obj;
            if (dictionary.TryGetValue(typeof(T), out obj))
            {
                return (Func<object, T>)obj;
            }
            return null;
        }

        private static class ConvertFuncHelper<T>
        {
            public static readonly Func<object, T> Default = DBReaderUtils.GetDefaultConvertFunc<T>();
        }
    }
}
