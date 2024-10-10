using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;

namespace WYF.DbEngine.db
{
    public abstract class AbstractDBImpl
    {
        private static ThreadLocal<AtomicBoolean> thQueryAlone = new ThreadLocal<AtomicBoolean>();

        public abstract QueryResult<T> Query<T>(DBRoute dbRoute, IDbConnection con, bool close, string sql, Func<IDataReader, T> action, bool convert, params object[] paramVarArgs);

        public IDataSet QueryDataSet(string algoKey, DBRoute dbRoute, string sql, object[] parameters, QueryMeta queryMeta, TraceSpan traceSpan)
        {
            //DataSet dataSet= DBUtils.ExecuteDataSet(new Context(), CommandType.Text, sql, null);

            IDataReader  dataReader= DBUtils.ExecuteReader(new Context(), sql, null);
            QueryMeta qm = QueryMeta.CreateOrFixQueryMeta(queryMeta, dataReader, DatabaseType.MS_SQL_Server);
            return WYF.Algo.Algo.Create(algoKey).CreateDataSet(dataReader, qm.RowMeta);
            //return dataSet;
            
           
        }
        public IDataReader QueryDataReader(string algoKey, DBRoute dbRoute, string sql, object[] parameters, QueryMeta queryMeta, TraceSpan traceSpan)
        {
            IDataReader dataReader= DBUtils.ExecuteReader(new Context(), sql, null);
            return dataReader;


        }
    }
}
