using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Utils;
using WYF.DbEngine;
using WYF.DbEngine.db;

namespace WYF.OrmEngine.sequence
{
    public class SequenceReader
    {


        private DBRoute dbRoute = null;
        private readonly Sequence sequence = IDServiceSequence.Instance;

        public SequenceReader()
        {
        }

        public SequenceReader(DBRoute dbRoute)
        {
            this.dbRoute = dbRoute;
        }

        public void AutoSetPrimaryKey(object[] dataEntities, IDataEntityType entityType)
        {
            AutoSetPrimaryKey(dataEntities.ToList(), entityType);
        }

        public void AutoSetPrimaryKey(List<object> dataEntities, IDataEntityType entityType)
        {
            Action<DataEntityWalkerEventArgs> callback = (e) =>
            {
                Type type = entityType.PrimaryKey.PropertyType;
                ISimpleProperty pkProperty = e.DataEntityType.PrimaryKey;
                if (pkProperty != null)
                    SetPrimaryKey<object>(e.DataEntities, pkProperty, e.DataEntityType.Alias);
            };

        

            OrmUtils.DataEntityWalker(dataEntities, entityType, callback, true);
        }


        private void SetPrimaryKey<T>(List<object> dataEntities, ISimpleProperty pkProperty, string tableName)
        {
            List<object> toDoList = new List<object>();
            foreach (object dataEntity in dataEntities)
            {
                if (dataEntity != null)
                {
                    T currentValue = (T)pkProperty.GetValueFast(dataEntity);
                    if (currentValue == null ||
                        (currentValue is string && string.IsNullOrEmpty(currentValue.ToNullString())) ||
                        (currentValue is int && currentValue.ToInt() == 0) ||
                        (currentValue is long && currentValue.ToDouble() == 0L)
                      
                        )
                    {
                        toDoList.Add(dataEntity);
                    }
                }
            }

            if (toDoList.Count > 0)
            {
                Type type = pkProperty.PropertyType;
                if (type == typeof(int))
                {
                    SetPropValue(toDoList, pkProperty, GetSequences<int>(new int[0], tableName, toDoList.Count));
                }
                else if (type == typeof(long))
                {
                    SetPropValue(toDoList, pkProperty, GetSequences<long>(new long[0], tableName, toDoList.Count));
                }
                else if (type == typeof(string))
                {
                    throw new NotImplementedException();
                   // SetPropValue(toDoList, pkProperty, GetStringSequence(toDoList.Count));
                }
                else if (type == typeof(Guid))
                {
                    SetPropValue(toDoList, pkProperty, GetGuidSequence(toDoList.Count));
                }
            }
        }


        /// <summary>
        /// 获取指定长度的UUID序列。
        /// </summary>
        /// <param name="len">序列长度</param>
        /// <returns>包含UUID的数组</returns>
        public static Guid[] GetGuidSequence(int len)
        {
            Guid[] ids = new Guid[len];
            for (int i = 0; i < len; i++)
            {
                ids[i] = Guid.NewGuid();
            }
            return ids;
        }

        public T[] GetSequences<T>(T[] a, string tableName, int count)
        {
            T[] iret = sequence.GetSequence(a, tableName, count);

            if (sequence.RepairMaxSeq())
            {
                if (typeof(T) == typeof(int))
                {
                    if ((int)(object)iret[0] == 100001)
                    {
                        RepairMaxSeq(ref iret, a, tableName, count);
                    }
                }
                else if (typeof(T) == typeof(long))
                {
                    if ((long)(object)iret[0] == 100001L)
                    {
                        RepairMaxSeq(ref iret, a, tableName, count);
                    }
                }

                return SetValueHandler.GetValue(a, iret);
            }

            return iret;
        }


        /// <summary>
        /// 修复最大序列值并获取新的序列值。
        /// </summary>
        /// <typeparam name="T">序列值的类型</typeparam>
        /// <param name="iret">结果数组引用</param>
        /// <param name="a">初始数组</param>
        /// <param name="key">键名</param>
        /// <param name="count">要获取的序列值数量</param>
        public void RepairMaxSeq<T>(ref T[] iret, T[] a, string key, int count)
        {
            if (ExistRecords(key))
            {
                string seqName = sequence.GetSequenceNameByKey(key);
                try
                {
                    sequence.RepairMaxValue(seqName, key, 100001L);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("repairMaxValue error", e);
                }

                iret = sequence.GetSequence(a, key, count);
            }
        }


        /// <summary>
        /// 检查表中是否存在记录。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>如果存在记录，返回true；否则返回false。</returns>
        private bool ExistRecords(string tableName)
        {
            string sql = "SELECT TOP 1 1 FROM " + tableName;
            return (bool)DB.Query(this.dbRoute, sql, null, 
                 rs =>
                {
                    if (rs.Read())
                    {
                        return rs.GetInt32(0) > 0;
                    }
                    return false;
                }
            );
        }

        private void SetPropValue<T>(List<object> toDoList, ISimpleProperty pkProperty, T[] pkValues)
        {
            for (int i = 0; i < toDoList.Count; i++)
            {
                pkProperty.SetValue(toDoList[i], pkValues[i]);
            }
        }

        private void SetPropValue(List<object> toDoList, ISimpleProperty pkProperty, string[] pkValues)
        {
            for (int i = 0; i < toDoList.Count; i++)
            {
                pkProperty.SetValue(toDoList[i], pkValues[i]);
            }
        }

        private void SetPropValue(List<object> toDoList, ISimpleProperty pkProperty, Guid[] pkValues)
        {
            for (int i = 0; i < toDoList.Count; i++)
            {
                pkProperty.SetValue(toDoList[i], pkValues[i]);
            }
        }


    }
}
