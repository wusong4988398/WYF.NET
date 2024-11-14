using FreeRedis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WYF.DbEngine;
using WYF.DbEngine.db;

namespace WYF.OrmEngine.sequence
{
    public abstract class Sequence
    {
        private const int SYSTEM_MAX_VALUE = 100000;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        protected string SqlSequenceObjectQuery;
        protected string CreateSql;
        protected string SqlSequenceQuery;
        protected string SqlTableQuery;
        protected string SqlPkQuery;
        protected DBRoute DbRoute;

        public Sequence(DBRoute dbRoute)
        {
            DbRoute = dbRoute;
        }

        /// <summary>
        /// 检查指定的表是否存在。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>如果表存在，返回true；否则返回false。</returns>
        public bool HasTableExists(string tableName)
        {
            return (bool)DB.Query(DbRoute, SqlTableQuery, [new SqlParam(":A", KDbType.String, tableName.ToUpper())], rs =>
            {
                if (!rs.Read())
                    return false;
                var count = rs.GetValue(0);
                return count != null && Convert.ToInt32(count) > 0;
            });
        }

        /// <summary>
        /// 获取表的最大序列值。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>表的最大序列值，默认为100000。</returns>
        public long GetTableMaxSeq(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                return 100000L;

            if (HasTableExists(tableName))
            {
                var pkField = DB.Query(DbRoute, SqlPkQuery, new object[] { tableName.ToUpper() },rs =>
                {
                    if (rs.Read())
                        return rs.GetValue(0);
                    return false;
                });

                if (pkField == null)
                    return 100000L;

                string sql = $"/*dialect*/ select max(t.{pkField}) from {tableName} t";
                var maxValue = DB.Query(DbRoute, sql, null, rs =>
                {
                    if (rs.Read())
                        return rs.GetValue(0);
                    return false;
                });

                if (maxValue == null || maxValue is string)
                    return 100000L;

                long maxValueLong = 100000L;
                if (long.TryParse(maxValue.ToString(), out long parsedValue))
                    maxValueLong = parsedValue;

                return Math.Max(maxValueLong, 100000L);
            }

            return 100000L;
        }

        /// <summary>
        /// 根据表名生成序列名。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>生成的序列名</returns>
        public string GetSequenceNameByKey(string tableName)
        {
            if (tableName.StartsWith("t_"))
                return "Z_" + tableName.Substring(2);
            return "Z_" + tableName;
        }

        /// <summary>
        /// 抽象方法，用于修复最大值。
        /// </summary>
        /// <param name="seqName">序列名</param>
        /// <param name="tableName">表名</param>
        /// <param name="maxValue">最大值</param>
        /// <exception cref="SqlException"></exception>
        public abstract void RepairMaxValue(string seqName, string tableName, long maxValue);

        /// <summary>
        /// 检查指定的序列是否存在。
        /// </summary>
        /// <param name="seqName">序列名</param>
        /// <returns>如果序列存在，返回true；否则返回false。</returns>
        public bool HasSequence(string seqName)
        {
            return (bool)DB.Query(DbRoute, SqlSequenceObjectQuery, [new SqlParam(":A", KDbType.String, seqName.ToUpper())], rs =>
            {
                if (!rs.Read())
                    return false;
                var count = rs.GetValue(0);
                return count != null && Convert.ToInt32(count) > 0;
            });
        }

        /// <summary>
        /// 自动创建序列。
        /// </summary>
        /// <param name="seqName">序列名</param>
        /// <param name="tableName">表名</param>
        public void AutoCreateSequence(string seqName, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("tableName is empty");

            if (seqName.Length > 30)
                throw new Exception( $"哦，真糟糕，为表{tableName}建立的序列{seqName}时发现表名称太长了（超过了30个字符），尝试在设计器中修订表名称。");

            _lock.EnterWriteLock();
            try
            {
                if (HasSequence(seqName))
                    return;

                long currentValue = GetTableMaxSeq(tableName) + 1L;
                string createSql = string.Format(CreateSql, seqName, currentValue);
                try
                {
                    DB.Execute(DbRoute, createSql, null);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error:{e.Message}. seqName:{seqName}");
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取序列值。
        /// </summary>
        /// <typeparam name="T">序列值的类型</typeparam>
        /// <param name="a">结果数组</param>
        /// <param name="key">键名</param>
        /// <param name="count">要获取的序列值数量</param>
        /// <returns>包含序列值的数组</returns>
        public virtual T[] GetSequence<T>(T[] a, string key, int count)
        {
            string seqName = GetSequenceNameByKey(key);
            string sqlSequenceQuery = string.Format(SqlSequenceQuery, seqName);

            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    SqlParam[] parameters = { new SqlParam(":count", KDbType.Int32, count) };

                    if (typeof(T) == typeof(int))
                    {
                        int startValue1 = (int)DB.Query(DbRoute, sqlSequenceQuery, parameters, rs =>
                        {
                            if (rs.Read())
                                return rs.GetInt32(0);
                            return 0;
                        });

                        int[] resultArray2 = new int[count];
                        for (int i = 0; i < count; i++)
                            resultArray2[i] = startValue1+ i;

                        transactionScope.Complete();
                        return (T[])(object)resultArray2;
                    }

                    long startValue = (long)DB.Query(DbRoute, sqlSequenceQuery, parameters, rs =>
                    {
                        if (rs.Read())
                            return rs.GetInt64(0);
                        return 0L;
                    });

                    long[] resultArray = new long[count];
                    for (int i = 0; i < count; i++)
                        resultArray[i] = startValue + i;

                    transactionScope.Complete();
                    return (T[])(object)resultArray;
                }
            }
            catch (Exception e)
            {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    try
                    {
                        AutoCreateSequence(seqName, key);
                    }
                    catch (Exception e2)
                    {
                        transactionScope.Dispose();
                        throw new Exception($"Error:{e.Message}");
                    }

                    transactionScope.Complete();
                    return GetSequence(a, key, count);
                }
            }
        }

        /// <summary>
        /// 修复序列值。
        /// </summary>
        /// <typeparam name="T">序列值的类型</typeparam>
        /// <param name="a">结果数组</param>
        /// <param name="key">键名</param>
        /// <param name="seqName">序列名</param>
        /// <param name="tableName">表名</param>
        /// <param name="isInt32">是否为32位整数</param>
        public void RepairSequence<T>(T[] a, string key, string seqName, string tableName, bool isInt32)
        {
            long seqValue;
            // 获取序列的第一个值
            T[] sequenceValues = GetSequence(a, key, 1);
            if (isInt32)
            {
                seqValue = Convert.ToInt64(Convert.ToInt32(sequenceValues[0]));
            }
            else
            {
                seqValue = Convert.ToInt64(sequenceValues[0]);
            }

            // 增加序列值
            seqValue++;


            try
            {
                RepairMaxValue(seqName, tableName, seqValue);
            }
            catch (Exception e)
            {
                throw new Exception($"Error:{e.Message}");
            }
        }

        /// <summary>
        /// 检查是否需要修复最大序列值。
        /// </summary>
        /// <returns>如果需要修复，返回true；否则返回false。</returns>
        public virtual bool RepairMaxSeq()
        {
            return true;
        }
    }
}
