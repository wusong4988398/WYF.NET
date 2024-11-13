using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.sequence
{
    public static class SetValueHandler
    {
        /// <summary>
        /// 将BigDecimal数组转换为int数组。
        /// </summary>
        /// <param name="values">BigDecimal数组</param>
        /// <returns>int数组</returns>
        public static int[] GetIntValue(decimal[] values)
        {
            int[] newArray = new int[values.Length];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = (int)values[i];
            }
            return newArray;
        }

        /// <summary>
        /// 将BigDecimal数组转换为long数组。
        /// </summary>
        /// <param name="values">BigDecimal数组</param>
        /// <returns>long数组</returns>
        public static long[] GetLongValue(decimal[] values)
        {
            long[] newArray = new long[values.Length];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = (long)values[i];
            }
            return newArray;
        }

        /// <summary>
        /// 将一个数组的值复制到另一个数组中。
        /// </summary>
        /// <typeparam name="T">数组元素的类型</typeparam>
        /// <param name="a">目标数组</param>
        /// <param name="iret">源数组</param>
        /// <returns>目标数组</returns>
        public static T[] GetValue<T>(T[] a, T[] iret)
        {
            a = new T[iret.Length];
            Array.Copy(iret, a, iret.Length);
            if (a.Length > iret.Length)
            {
                a[iret.Length] = default(T);
            }
            return a;
        }
    }
}
