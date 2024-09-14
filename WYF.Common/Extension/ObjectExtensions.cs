using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public static class ObjectExtensions
    {

        #region CopyPropertiesTo
        public static void CopyPropertiesTo(this object source, object destination, OptionTyp options = OptionTyp.None)
        {
            SetProperties(GetProperties(source), destination, options);
        }
        public static void CopyPropertiesWithMap(object pobSrc, object pobDest, Dictionary<string, string> pdiMap, OptionTyp penOpt)
        {
            List<string> strSrc = new List<string>();
            List<string> strDest = new List<string>();
            foreach (KeyValuePair<string, string> pair in pdiMap)
            {
                strSrc.Add(pair.Key);
                strDest.Add(pair.Value);
            }
            CopyPropertiesWithMap(pobSrc, pobDest, strSrc.ToArray(), strDest.ToArray(), penOpt);
        }

        public static void CopyPropertiesWithMap(object pobSrc, object pobDest, string[] pstSrcPropertyNames, string[] pstDestPropertyNames, OptionTyp penOpt)
        {
            if (null == pobSrc || null == pobDest)
            { throw new ArgumentNullException("one of the arguments is null!"); }

            if (pstDestPropertyNames.Length != pstSrcPropertyNames.Length)
                throw new ArgumentException("pstDestPropertyNames & pstSrcPropertyNames must have same length");

            for (int i = 0; i < pstDestPropertyNames.Length; i++)
            {
                CopyProperty(pobSrc, pobDest, pstSrcPropertyNames[i], pstDestPropertyNames[i], penOpt);
            }
        }

        public static T GenernationObject<T>(object pobSrc, OptionTyp penOpt)
        {
            T lobDest = Activator.CreateInstance<T>();
            pobSrc.CopyPropertiesTo(lobDest, penOpt);
            return lobDest;
        }

        public static T GenernationObject<T>(Dictionary<string, object> pdiProperties, OptionTyp penOpt)
        {
            T lobDest = Activator.CreateInstance<T>();
            SetProperties(pdiProperties, lobDest, penOpt);

            return lobDest;
        }

        public static Dictionary<string, object> GetProperties(object pobObj)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            string name;
            object val;

            if (null == pobObj) { throw new ArgumentNullException("pobObj can't be null"); }

            Type objType = pobObj.GetType();

            if (pobObj is Dictionary<string, object>)
            {
                Dictionary<string, object> keyValuePairs = pobObj as Dictionary<string, object>;
                foreach (var item in keyValuePairs)
                {

                    list.Add(item.Key, item.Value);
                }
            }
            else
            {
                PropertyInfo[] objInfo = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                for (int i = 0; i < objInfo.Length; i++)
                {
                    name = objInfo[i].Name;
                    val = objInfo[i].GetValue(pobObj, null);

                    list.Add(name, val);
                }
            }


            return list;
        }

        public static void SetProperties(Dictionary<string, object> pdiProperties, object pobObj, OptionTyp penOpt)
        {
            foreach (KeyValuePair<string, object> pair in pdiProperties)
            {
                try
                {
                    SetProperty(pobObj, pair.Key, pair.Value, penOpt);
                }
                catch (Exception) { }
            }
        }

        public static void CopyProperty(object pobSrc, object pobDest, string pstPropertyName, OptionTyp penOpt)
        {
            CopyProperty(pobSrc, pobDest, pstPropertyName, pstPropertyName, penOpt);
        }

        public static void CopyProperty(object pobSrc, object pobDest, string pstSrcPropertyName, string pstDestPropertyName, OptionTyp penOpt)
        {
            SetProperty(pobDest, pstDestPropertyName, GetProperty(pobSrc, pstSrcPropertyName, penOpt), penOpt);
        }

        public static void SetProperty(object pobObj, string pstPropertyName, object pobValue, OptionTyp penOpt)
        {
            if (null == pobObj || string.IsNullOrEmpty(pstPropertyName))
            {
                throw new ArgumentNullException("one of the arguments is null!");
            }

            bool isIgnoreCase = ((penOpt & OptionTyp.IsIgnoreCase) == OptionTyp.IsIgnoreCase);
            bool isConvert = ((penOpt & OptionTyp.IsConvert) == OptionTyp.IsConvert);
            bool isThrowConvertException = ((penOpt & OptionTyp.IsThrowConvertException) == OptionTyp.IsThrowConvertException);

            Type t = pobObj.GetType();
            PropertyInfo objInfo = null;
            if (isIgnoreCase)
            {
                PropertyInfo[] objInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo p in objInfos)
                {
                    if (p.Name.ToUpperInvariant().Equals(pstPropertyName.ToUpperInvariant()))
                    {
                        objInfo = p;
                        break;
                    }
                }
            }
            else
                objInfo = t.GetProperty(pstPropertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == objInfo)
                throw new Exception("none mapping property");

            object descVal = null;
            if (null == pobValue || !isConvert)
                descVal = pobValue;
            else

                descVal = pobValue.ChangeType(objInfo.PropertyType);
            //descVal = GetDestValue(pobValue.GetType(), objInfo.PropertyType, pobValue, isThrowConvertException);



            objInfo.SetValue(pobObj, descVal, null);
        }

        private static object GetDestValue(Type type, Type propertyType, object pobValue, bool isThrowConvertException)
        {
            return pobValue;
        }

        public static object GetProperty(object pobObj, string pstPropertyName, OptionTyp penOpt)
        {
            if (null == pobObj || string.IsNullOrEmpty(pstPropertyName))
            {
                throw new ArgumentNullException("Argument can't be null!");
            }
            bool isIgnoreCase = ((penOpt & OptionTyp.IsIgnoreCase) == OptionTyp.IsIgnoreCase);
            Type t = pobObj.GetType();
            PropertyInfo objInfo = null;
            if (isIgnoreCase)
            {
                PropertyInfo[] objInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo p in objInfos)
                {
                    if (p.Name.ToUpperInvariant().Equals(pstPropertyName.ToUpperInvariant()))
                    {
                        objInfo = p;
                        break;
                    }
                }
            }
            else
                objInfo = t.GetProperty(pstPropertyName, BindingFlags.Instance | BindingFlags.Public);

            if (null == objInfo)
                throw new Exception("none mapping property");

            object val = objInfo.GetValue(pobObj, null);
            return val;
        }
        #endregion

        /// <summary>
        /// 将一个对象转换为指定类型
        /// </summary>
        /// <param name="obj">待转换的对象</param>
        /// <param name="type">目标类型</param>
        /// <returns>转换后的对象</returns>
        public static object ChangeType(this object obj, Type type)
        {
            if (type == null) return obj;
            if (type == typeof(string)) return obj?.ToString();
            if (type == typeof(Guid) && obj != null) return Guid.Parse(obj.ToString());
            if (obj == null) return type.IsValueType ? Activator.CreateInstance(type) : null;

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType())) return obj;
            else if ((underlyingType ?? type).IsEnum)
            {
                if (underlyingType != null && string.IsNullOrWhiteSpace(obj.ToString())) return null;
                else return Enum.Parse(underlyingType ?? type, obj.ToString());
            }
            // 处理DateTime -> DateTimeOffset 类型
            else if (obj.GetType().Equals(typeof(DateTime)) && (underlyingType ?? type).Equals(typeof(DateTimeOffset)))
            {
                return ((DateTime)obj).ConvertToDateTimeOffset();
            }
            // 处理 DateTimeOffset -> DateTime 类型
            else if (obj.GetType().Equals(typeof(DateTimeOffset)) && (underlyingType ?? type).Equals(typeof(DateTime)))
            {
                return ((DateTimeOffset)obj).ConvertToDateTime();
            }
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return underlyingType == null ? Activator.CreateInstance(type) : null;
                }
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(obj.GetType())) return converter.ConvertFrom(obj);

                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    var o = constructor.Invoke(null);
                    var propertys = type.GetProperties();
                    var oldType = obj.GetType();

                    foreach (var property in propertys)
                    {
                        var p = oldType.GetProperty(property.Name);
                        if (property.CanWrite && p != null && p.CanRead)
                        {
                            property.SetValue(o, ChangeType(p.GetValue(obj, null), property.PropertyType), null);
                        }
                    }
                    return o;
                }
            }
            return obj;
        }


        /// <summary>
        /// 将一个对象转换为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ChangeType<T>(this object obj)
        {
            return (T)ChangeType(obj, typeof(T));
        }
        public static bool IsEmpty(this object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 将 DateTimeOffset 转换成本地 DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return dateTime.ToLocalTime().DateTime;
            else
                return dateTime.DateTime;
        }

        /// <summary>
        /// 将 DateTime 转换成 DateTimeOffset
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeOffset ConvertToDateTimeOffset(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }
    }

    [Flags, Serializable]
    public enum OptionTyp
    {
        None = 0,
        IsIgnoreCase = 1,
        IsConvert = 2,
        IsThrowConvertException = 3
    }
}
