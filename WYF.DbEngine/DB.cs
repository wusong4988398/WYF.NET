using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine.db;

namespace WYF.DbEngine
{
    public class DB
    {
        private static ThreadLocal<AbstractDBImpl> thImpl = new ThreadLocal<AbstractDBImpl>();
        private static AbstractDBImpl defaultImpl= GetDefaultImplInstance();

        private static AbstractDBImpl GetDefaultImplInstance()
        {
            return DBImpl.GetInstance();
        }

        public static Object Instance
        {
            
            // get { return SqlsugarHelper.Init(); }
            get { return null; }
        }
        //public static T Query<T>(DBRoute dbRoute, string sql, object[] paramss, ResultSetHandler<T> rh)
        //{
        //    ITraceSpan ts = null;

        //    return GetImpl().Query(dbRoute, sql, paramss, rh, ts);
        //}

        private static AbstractDBImpl GetImpl()
        {
            AbstractDBImpl threadDB = thImpl.Value;
            AbstractDBImpl ret = (threadDB == null) ? defaultImpl : threadDB;
            //if (ret is XDBImpl)
            //ShardingManager.get().loadShardingConfigs(false);
            return ret;
        }

        public static int[] ExecuteBatch(DBRoute dbRoute, String sql, List<Object[]> paramsList)
        {
            //TraceSpan ts = new TraceSpan();
            //return GetImpl().ExecuteBatch(dbRoute, sql, paramsList, ts);
            return null;
        }

        public static T Query<T>(DBRoute dbRoute, SqlBuilder sb, Func<IDataReader, T> action)
        {
            //return ExecuteSqlBuilder<T>(dbRoute, sb, (span, db, sqlobj) =>
            //{

            //    return db.Query<T>(dbRoute, sqlobj.Sql.ToString(), sqlobj.Parameters.ToArray(), action, span);

            //});
            return default;
        }

        //private static T ExecuteSqlBuilder<T>(DBRoute dbRoute, SqlBuilder sb, Func<TraceSpan, AbstractDBImpl, SqlObject, T> cb)
        //{
        //    AbstractDBImpl db = GetImpl();
        //    TraceSpan ts = new TraceSpan();

        //    SqlObject so = sb.GenSQLObject(null);
        //    return cb.Invoke(ts, db, so);

        //}

        public static DataSet QueryDataSet(string algoKey, DBRoute dbRoute, string sql, Object[] paramter, QueryMeta queryMeta)
        {

            TraceSpan ts = new TraceSpan();

            return GetImpl().QueryDataSet(algoKey, dbRoute, sql, paramter, queryMeta, ts);

        }
    }
}
