using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine;

namespace WYF.OrmEngine.sequence
{
    public class IDServiceSequence : Sequence
    {
        public static readonly bool EnableIDService = true;
        public static readonly IDServiceSequence Instance = new IDServiceSequence();

        private IDServiceSequence() : base(null)
        {
           
        }

        /// <summary>
        /// 获取指定类型的序列值。
        /// </summary>
        /// <typeparam name="T">序列值的类型</typeparam>
        /// <param name="emptyArrayForType">用于确定类型的空数组</param>
        /// <param name="tableName">表名</param>
        /// <param name="count">要获取的序列值数量</param>
        /// <returns>包含序列值的数组</returns>
        public override T[] GetSequence<T>(T[] emptyArrayForType, string tableName, int count)
        {
            Type type = typeof(T);
            if (type == typeof(int))
            {
                throw new NotImplementedException();
                //int[] intIds = DB.GenIntIds(tableName, count);
                //T[] result = new T[count];
                //for (int j = 0; j < count; j++)
                //{
                //    result[j] = (T)(object)intIds[j];
                //}
                //return result;
            }
            else
            {
                long[] longIds = DB.GenLongIds(tableName, count);
                T[] result = new T[count];
                for (int i = 0; i < count; i++)
                {
                    result[i] = (T)(object)longIds[i];
                }
                return result;
            }
        }

        /// <summary>
        /// 修复最大序列值。
        /// </summary>
        /// <returns>始终返回false</returns>
        public override bool RepairMaxSeq()
        {
            return false;
        }

        /// <summary>
        /// 修复最大值。
        /// </summary>
        /// <param name="seqName">序列名</param>
        /// <param name="tableName">表名</param>
        /// <param name="seqValue">最大值</param>
        public override void RepairMaxValue(string seqName, string tableName, long seqValue)
        {
            // 实现具体的修复逻辑
        }
    }

}
