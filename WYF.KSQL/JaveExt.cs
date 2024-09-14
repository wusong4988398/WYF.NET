using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL
{
    internal static class JaveExt
    {
        // Methods
        public static void add(this Dictionary<string, string> dt, string key, string value)
        {
            if (dt.ContainsKey(key))
            {
                dt[key] = value;
            }
            else
            {
                dt.Add(key, value);
            }
        }

        public static char charAt(this StringBuilder sb, int paramInt)
        {
            if ((paramInt < 0) || (paramInt >= sb.Length))
            {
                throw new IndexOutOfRangeException();
            }
            return sb[paramInt];
        }

        public static char CharAt(this string str, int i)
        {
            return str[i];
        }

        public static bool endsWith(this StringBuilder sb, char paramChar)
        {
            int length = sb.Length;
            return ((length > 0) && (sb[length - 1] == paramChar));
        }

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        public static int indexOf(this string s, int paramInt1, int paramInt2)
        {
            int length = s.Length;
            char[] chArray = s.ToCharArray(0, s.Length - 1);
            if (paramInt2 < 0)
            {
                paramInt2 = 0;
            }
            else if (paramInt2 >= s.Length)
            {
                return -1;
            }
            int index = paramInt2;
            if (paramInt1 < 0x10000)
            {
                while (index < length)
                {
                    if (chArray[index] == paramInt1)
                    {
                        return index;
                    }
                    index++;
                }
                return -1;
            }
            if (paramInt1 <= 0x10ffff)
            {
                char[] chArray2 = toChars(paramInt1);
                while (index < length)
                {
                    if (chArray[index] == chArray2[0])
                    {
                        if ((index + 1) == length)
                        {
                            break;
                        }
                        if (chArray[index + 1] == chArray2[1])
                        {
                            return index;
                        }
                    }
                    index++;
                }
            }
            return -1;
        }

        public static StringBuilder replace(this StringBuilder source, int start, int len, string newstring)
        {
            return source.Replace(source.ToString(), newstring, start, len);
        }

        public static void setCharAt(this StringBuilder sb, int paramInt, char paramChar)
        {
            if ((paramInt < 0) || (paramInt >= sb.Length))
            {
                throw new IndexOutOfRangeException();
            }
            sb[paramInt] = paramChar;
        }

        public static bool startsWith(this string str, char paramChar)
        {
            return (((str != null) && (str.Length > 0)) && (str[0] == paramChar));
        }

        public static string substring(this string source, int start, int end)
        {
            string str = string.Empty;
            str = source.Substring(0, end);
            return str.Substring(start, str.Length - start);
        }

        public static string substring(this StringBuilder sb, int paramInt1, int paramInt2)
        {
            if (paramInt1 < 0)
            {
                throw new IndexOutOfRangeException("Start index Out Of Range");
            }
            if (paramInt2 > sb.Length)
            {
                throw new IndexOutOfRangeException("End index Out Of Max Range");
            }
            if (paramInt1 > paramInt2)
            {
                throw new IndexOutOfRangeException("Start index Greater End index");
            }
            return sb.ToString().substring(paramInt1, paramInt2);
        }

        public static char[] toChars(int paramInt)
        {
            if ((paramInt < 0) || (paramInt > 0x10ffff))
            {
                throw new AggregateException();
            }
            if (paramInt < 0x10000)
            {
                return new char[] { ((char)paramInt) };
            }
            char[] paramArrayOfChar = new char[2];
            toSurrogates(paramInt, paramArrayOfChar, 0);
            return paramArrayOfChar;
        }

        private static void toSurrogates(int paramInt1, char[] paramArrayOfChar, int paramInt2)
        {
            int num = paramInt1 - 0x10000;
            paramArrayOfChar[paramInt2 + 1] = (char)((num & 0x3ff) + 0xdc00);
            paramArrayOfChar[paramInt2] = (char)((num >> 10) + 0xd800);
        }
    }






}
