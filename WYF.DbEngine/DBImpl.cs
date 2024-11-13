using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine.db;

namespace WYF.DbEngine
{
    public class DBImpl : AbstractDBImpl
    {
        private static  DBImpl self = new DBImpl();

        public static DBImpl GetInstance()
        {
            return self;
        }

        /// <summary>
        /// 执行SQL语句并返回是否成功。
        /// </summary>
        /// <param name="dbRoute">数据库路由</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="params">参数数组</param>
        /// <param name="ts">跟踪跨度</param>
        /// <returns>如果执行成功且至少有一行受影响，返回true；否则返回false。</returns>
        public override bool Execute(DBRoute dbRoute, string sql, object[] @params, TraceSpan ts)
        {
            List<object[]> paramsList;
            if (@params != null)
            {
                paramsList = new List<object[]> { @params };
            }
            else
            {
                paramsList = new List<object[]>();
            }

            int[] updateCounts = ExecuteBatch(dbRoute, sql, paramsList, ts);

            foreach (int count in updateCounts)
            {
                if (count > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public int[] ExecuteBatch(DBRoute dbRoute, String sql, List<Object[]> paramsList, TraceSpan ts)
        {
            return ExecuteBatch(dbRoute, GetConnection(dbRoute, sql, ts), true, sql, paramsList);
        }

        private int[] ExecuteBatch(DBRoute dbRoute, SqlSugarProvider dbProvider, bool v, string sql, List<object[]> paramsList)
        {
            return [dbProvider.Ado.ExecuteCommand(sql, paramsList)];
        }

        public override QueryResult<T> Query<T>(DBRoute dbRoute, bool close, string sql, Func<IDataReader, T> action, bool convert, params object[] parameters)
        {
            IDataReader rs = DBUtils.ExecuteReader(new Context(), sql, parameters);
            bool rollback = false;
            QueryResource resource = new QueryResource();
            //IDbCommand dbcommand = con.CreateCommand();
            //dbcommand.CommandText = sql;
            //dbcommand.CommandType = CommandType.Text;
            //IDataReader rs = dbcommand.ExecuteReader();
            T result = action.Invoke(rs);
            QueryResult<T> queryResult = new QueryResult<T>(result, resource);
            return queryResult;
        }

        public override int Update(DBRoute dbRoute, string sql, object[] parameters, TraceSpan ts)
        {
            return Update(dbRoute, GetConnection(dbRoute, sql, ts), true, sql, parameters);
        }

        private int Update(DBRoute dbRoute, SqlSugarProvider sqlSugarProvider, bool v, string sql, object[] parameters)
        {
            return sqlSugarProvider.Ado.ExecuteCommand(sql, parameters);
        }
    }
}
