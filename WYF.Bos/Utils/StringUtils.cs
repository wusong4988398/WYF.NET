
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WYF;


namespace WYF.Bos.Utils
{
    public static class StringUtils
    {

        private static Regex CntTypReg = new Regex(";{0,}json;{0,}");

        private static long ptick = 0L;
        public static char CharAt(this string str, int i)
        {
            return str[i];
        }

        public static bool CheckContentType(this string contentType, string param)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return (contentType == param);
            }
            if (string.IsNullOrWhiteSpace(param))
            {
                return (contentType == param);
            }
            string input = contentType.ToLowerInvariant();
            string str2 = param.ToLowerInvariant();
            return Regex.IsMatch(input, ";{0,}" + str2 + ";{0,}");
        }
        public static bool CheckSqlValidateForCloudErp(string strsql)
        {
            bool result = true;
            List<string> list = new List<string>();
            list.Add("insert ");
            list.Add("update ");
            list.Add("delete ");
            for (int i = 0; i < list.Count; i++)
            {
                if (strsql.IndexOf(list[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        public static bool CheckContentTypeIsDesktopApp(this string contentType)
        {
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                if (contentType.StartsWith("application/x-www-form-urlencoded"))
                {
                    return true;
                }
                if (contentType.StartsWith("multipart/form-data;boundary="))
                {
                    return true;
                }
            }
            return false;
        }


        public static string GetSqlWithCardinality(int count, string param, int type, bool IsContainC = true)
        {
            string text = string.Format(" select /*+ cardinality(b {0})*/ FID  from table(fn_StrSplit({1},',',{2})) b ", count, param, type);
            if (count < 0)
            {
                string arg = "{" + -count + "}";
                text = string.Format(" select /*+ cardinality(b {0})*/ FID  from table(fn_StrSplit({1},',',{2})) b ", arg, param, type);
            }
            if (IsContainC)
            {
                text = " ( " + text + " ) ";
            }
            return text;
        }

        public static bool CheckContentTypeWithJson(this string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }
            string input = contentType.ToLowerInvariant();
            return ((contentType == "json") || CntTypReg.IsMatch(input));
        }

        public static bool CheckSqlValidate(string strsql)
        {
            List<string> list = new List<string> {
            "drop database", "truncate table", "drop table", "truncate ", "create ", "drop ", "grant ", "sys.", "dbo.", "use master", "master.", "use tempdb", "tempdb.", "use ", "t_sec_user", "t_sec_fieldpermission",
            "t_sec_fieldpermission_l", "t_sec_fieldpermissionentry", "t_sec_funcpermission", "t_sec_funcpermission_l", "t_sec_funcpermissiondata", "t_sec_funcpermissionentry", "t_sec_datarule", "t_sec_datarule_l"
         };
            for (int i = 0; i < list.Count; i++)
            {
                if (strsql.IndexOf(list[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckUserAgent(this string userAgent, string param)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return (userAgent == param);
            }
            if (string.IsNullOrWhiteSpace(param))
            {
                return (userAgent == param);
            }
            string input = userAgent.ToLowerInvariant();
            string str2 = param.ToLowerInvariant();
            return Regex.IsMatch(input, "\b{0,}" + str2 + "\b{0,}");
        }

        public static string CustomReplace(string source, string subSrc, string subDest, char tag)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            int length = subSrc.Length;
            int startIndex = 0;
            while (startIndex < source.Length)
            {
                char ch = source[startIndex];
                if (ch == tag)
                {
                    flag = !flag;
                }
                if (!flag)
                {
                    if ((source.Length - startIndex) < length)
                    {
                        builder.Append(source.Substring(startIndex));
                        break;
                    }
                    if (string.Compare(source, startIndex, subSrc, 0, length) == 0)
                    {
                        builder.Append(subDest);
                        startIndex += length;
                        continue;
                    }
                }
                builder.Append(ch);
                startIndex++;
            }
            return builder.ToString();
        }

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static string EscapeXMLStr(string input)
        {
            if (input == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder("");
            int length = input.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = input.CharAt(i);
                switch (ch)
                {
                    case '&':
                        builder.Append("&amp;");
                        break;

                    case '<':
                        builder.Append("&lt;");
                        break;

                    case '>':
                        builder.Append("&gt;");
                        break;

                    case '\'':
                        builder.Append("&apos;");
                        break;

                    case '"':
                        builder.Append("&quot;");
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            return builder.ToString();
        }

        public static string FixedOrcalXMLString(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.Replace("&amp;", "&");
                str = str.Replace("&lt;", "<");
                str = str.Replace("&gt;", ">");
                str = str.Replace("&quot;", "\"");
                str = str.Replace("&apos;", "'");
            }
            return str;
        }

        public static string GetRandomString()
        {
            if (ptick > 0xe8d4a51000L)
            {
                ptick = 0L;
            }
            ptick += 1L;
            Random random = new Random((int)(ptick & 0xfffffffffffffffL));
            return random.Next().ToString();
        }

        public static string GetString(this string sString, int iPosition, string sP)
        {
            if (sString != null)
            {
                string[] separator = new string[] { sP };
                string[] strArray2 = sString.Split(separator, StringSplitOptions.None);
                if ((iPosition <= strArray2.Length) && (iPosition > 0))
                {
                    return strArray2[iPosition - 1];
                }
            }
            return "";
        }

        public static string GetStringEx(this string sString, string sSegment, string sP)
        {
            if (sString.Length != 0)
            {
                string[] separator = new string[] { sP };
                string[] strArray2 = sString.Split(separator, StringSplitOptions.None);
                char[] chArray = new char[] { '=' };
                foreach (string str in strArray2)
                {
                    string[] strArray3 = str.Split(chArray);
                    if (strArray3[0].Trim().Equals(sSegment, StringComparison.OrdinalIgnoreCase))
                    {
                        if (strArray3.GetUpperBound(0) < 1)
                        {
                            return "";
                        }
                        return strArray3[1].Trim();
                    }
                }
            }
            return "";
        }

        public static bool Inside(this string key, string[] value)
        {
            foreach (string str in value)
            {
                if (key.EqualsIgnoreCase(str))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDate(this string sInput)
        {
            DateTime time;
            return DateTime.TryParse(sInput, out time);
        }

        public static bool IsEmpty(this string str)
        {
            return ((str == null) || (str.Trim().Length <= 0));
        }

        public static bool IsInt(this string value)
        {
            if (value.Length == 0)
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        public static bool IsNumberAndLetters(this string str)
        {
            if (str == null)
            {
                return false;
            }
            if (str.Trim().Length <= 0)
            {
                return false;
            }
            return Regex.IsMatch(str, "^[A-Za-z0-9]+$");
        }

        public static bool IsNumeric(this string value)
        {
            if (value.Length == 0)
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        public static bool IsUnsign(this string value)
        {
            if (value.Length == 0)
            {
                return false;
            }
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }

        public static string JoinFilterString(this string str1, string str2, string separator = "AND")
        {
            if (string.IsNullOrWhiteSpace(str1))
            {
                return str2;
            }
            if (string.IsNullOrWhiteSpace(str2))
            {
                return str1;
            }
            return string.Format("(({0}) {2} ({1}))", str1, str2, separator);
        }

        public static int LengthOfChar(this string str)
        {
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (Encoding.Default.GetBytes(str.Substring(i, 1)).Length > 1)
                {
                    num += 2;
                }
                else
                {
                    num++;
                }
            }
            return num;
        }

        public static string ReplaceFirst(this string input, string oldValue, string newValue)
        {
            Regex regex = new Regex(oldValue, RegexOptions.Multiline);
            return regex.Replace(input, newValue, 1);
        }

        public static string Simplified2Traditional(string text)
        {
            return Strings.StrConv(text, VbStrConv.TraditionalChinese, 9);
        }

        public static string[] SplitRegex(this string str1, string strRegex)
        {
            Regex regex = new Regex(strRegex);
            return regex.Split(str1);
        }

        public static string[] SplitRegex(this string str1, string strRegex, int limit)
        {
            Regex regex = new Regex(strRegex);
            return regex.Split(str1, limit);
        }

        public static string ToHex(byte[] bytes)
        {
            string str = string.Empty;
            if ((bytes != null) || (bytes.Length > 0))
            {
                foreach (byte num in bytes)
                {
                    str = str + string.Format("{0:X2}", num);
                }
            }
            return str;
        }

        public static List<int> ToIntList(this string input, char splitChar)
        {
            List<int> list = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                foreach (string str in input.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int result = 0;
                    if (int.TryParse(str, out result))
                    {
                        list.Add(result);
                    }
                }
            }
            return list;
        }

        public static JSONArray ToJSONArray(string parameters)
        {
        
            return JsonConvert.DeserializeObject<JSONArray>(parameters);
        }

        public static JSONObject ToJSONObject(string parameters)
        {
       
            return JsonConvert.DeserializeObject<JSONObject>(parameters);
        }

        public static string Traditional2Simplified(string text)
        {
            return Strings.StrConv(text, VbStrConv.SimplifiedChinese, 9);
        }

        public static T[] Union<T>(IEnumerable<T[]> lists)
        {
            HashSet<T> source = new HashSet<T>();
            foreach (T[] localArray in lists)
            {
                foreach (T local in localArray)
                {
                    source.Add(local);
                }
            }
            return source.ToArray<T>();
        }

     
    

 
    }
}
