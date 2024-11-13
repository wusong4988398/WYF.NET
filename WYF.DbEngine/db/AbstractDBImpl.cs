using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using static IronPython.Modules.PythonSocket;

namespace WYF.DbEngine.db
{
    public abstract class AbstractDBImpl
    {
        private static ThreadLocal<AtomicBoolean> thQueryAlone = new ThreadLocal<AtomicBoolean>();

        public abstract QueryResult<T> Query<T>(DBRoute dbRoute, bool close, string sql, Func<IDataReader, T> action, bool convert, params object[] paramVarArgs);

        public IDataSet QueryDataSet(string algoKey, DBRoute dbRoute, string sql, object[] parameters, QueryMeta queryMeta, TraceSpan traceSpan)
        {
            //DataSet dataSet= DBUtils.ExecuteDataSet(new Context(), CommandType.Text, sql, null);

            IDataReader  dataReader= DBUtils.ExecuteReader(new Context(), sql, parameters);
            QueryMeta qm = QueryMeta.CreateOrFixQueryMeta(queryMeta, dataReader, DatabaseType.MS_SQL_Server);
            return WYF.Algo.Algo.Create(algoKey).CreateDataSet(dataReader, qm.RowMeta);
            //return dataSet;


        }

        protected SqlSugarProvider GetConnection( DBRoute dbRoute, string sql,  TraceSpan ts)
        {
            return SugarBase.Instance.GetConnection(dbRoute.RouteKey);
        }

        public T Query<T>(DBRoute dbRoute, string sql, object[] parameters, Func<IDataReader, T> action, TraceSpan ts)
        {
            QueryResult<T> ret = this.Query(dbRoute,true, sql, action,true, parameters);   
            return ret.Result;
        }

        public abstract bool Execute(DBRoute dbRoute, string sql, object[] parameters, TraceSpan ts);
        public abstract int Update(DBRoute dbRoute, string sql, object[] parameters, TraceSpan ts);
    }
}
