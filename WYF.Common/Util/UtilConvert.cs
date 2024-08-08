using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    /// <summary>
    /// 转换类
    /// </summary>
    public static class UtilConvert
    {
        /// <summary>
        /// 转整形
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static int ToInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue == null)
            {
                return 0;
            }

            if (thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }

            return reval;
        }


        /// <summary>
        /// 转非空字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToNullString(this object obj)
        {
            return (((obj == null) && (obj != DBNull.Value)) ? string.Empty : obj.ToString().Trim());
        }


        /// <summary>
        /// 转整形
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static int ToInt(this object thisValue, int errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out int reval))
            {
                return reval;
            }

            return errorValue;
        }

        /// <summary>
        /// 转金钱
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static double ToMoney(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value &&
                double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }

            return 0;
        }

        /// <summary>
        /// 转金钱
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static double ToMoney(this object thisValue, double errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value &&
                double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }

            return errorValue;
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string OBJToString(this object thisValue)
        {
            if (thisValue != null)
            {
                return thisValue.ToString().Trim();
            }

            return "";
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static string OBJToString(this object thisValue, string errorValue)
        {
            if (thisValue != null)
            {
                return thisValue.ToString().Trim();
            }

            return errorValue;
        }

        /// <summary>
        /// 转数字
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value &&
                decimal.TryParse(thisValue.ToString(), out decimal reval))
            {
                return reval;
            }


            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static double ToDouble(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value &&
                double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }


            return 0;
        }

        /// <summary>
        /// 转数字
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object thisValue, decimal errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value &&
                decimal.TryParse(thisValue.ToString(), out decimal reval))
            {
                return reval;
            }

            return errorValue;
        }

        /// <summary>
        /// 转日期
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object thisValue)
        {
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                reval = Convert.ToDateTime(thisValue);
            }

            return reval;
        }

        /// <summary>
        /// 转日期
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object thisValue, DateTime errorValue)
        {
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }

            return errorValue;
        }

        /// <summary>
        /// 转布尔
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool ToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }

            return reval;
        }


        /// <summary>
        /// 转SQL字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToSqlInStr(this List<int> list)
        {
            if (list != null)
            {
                return string.Join(",", list);
            }

            return "";
        }

        /// <summary>
        /// 转SQL字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToSqlInStr(this List<string> list)
        {
            if (list != null)
            {
                return "'" + string.Join("','", list) + "'";
            }

            return "";
        }
    }
}
