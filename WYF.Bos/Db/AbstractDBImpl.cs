using WYF.Bos.algo;
using WYF.Bos.db.tx;
using WYF.Bos.Threading;
using WYF.Bos.trace;
using WYF.Bos.xdb.tablemanager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public abstract class  AbstractDBImpl
    {
        private static ThreadLocal<AtomicBoolean> thQueryAlone = new ThreadLocal<AtomicBoolean>();

        public DataSet QueryDataSet(string algoKey, DBRoute dbRoute, string sql, object[] parameters, QueryMeta queryMeta, TraceSpan ts)
        {
            //DelegateConnection con = GetConnection(dbRoute, sql, ts);
            //QueryResult<IDataSet> ret = Query(dbRoute, con, false, sql, rs =>
            //{
            //    QueryMeta qm = QueryMeta.CreateOrFixQueryMeta(queryMeta, rs, con.GetDBConfig().GetDBType());
            //    rs = qm.ConvertResultSet(rs);
            //    return Algo.Create(algoKey).CreateDataSet(rs, qm.GetRowMeta());
            //}, false, parameters);
            //IDataSet ds = ret.GetResult();
            //DataSet ds= SqlsugarHelper._Instance.Ado.GetDataSetAll(sql, parameters);


            //return ds;
            return null;
        }
          public  T Query<T>(DBRoute dbRoute, String sql, Object[] parameters, Func<IDataReader,T> action, TraceSpan ts)
        {
            QueryResult<T> ret = Query(dbRoute, GetConnection(dbRoute, sql, ts), true, sql, action, true, parameters);
            if (ret.Resource != null)
            {
                //ret.Resource.CloseWithConnection();
            }

            return ret.Result;
        }

        public  IDbConnection GetConnection(DBRoute dbRoute, string sql, TraceSpan ts)
        {
            //SqlSugarProvider provider= SqlsugarHelper._Instance.GetConnection("0");
            //return provider;
            return null;
        }

        public abstract QueryResult<IDataSet> Query(DBRoute dbRoute, DelegateConnection con, bool close, string sql, Func<object, object> callback, bool convert, params object[] parameters);
        public abstract QueryResult<T> Query<T>(DBRoute dbRoute, IDbConnection con, bool close, string sql, Func<IDataReader,T> action, bool convert,  params object[] paramVarArgs);



        public abstract int[] ExecuteBatch(DBRoute dbRoute, String sql, List<Object[]> paramsList, TraceSpan ts);
        //T Query<T>(DBRoute dbRoute, String sql, Object[] paramss, ResultSetHandler<T> rh, ITraceSpan ts)
        //{
        //    //QueryResult<T> ret = Query(dbRoute, GetConnection(dbRoute, sql, ts), true, sql, rh, true, paramss);
        //    //if (ret.getResource() != null)
        //    //    ret.getResource().closeWithConnection();
        //    //return ret.getResult();
        //    return null;
        //}

        //protected DelegateConnection GetConnection( DBRoute dbRoute,  String sql,  ITraceSpan ts)
        //{
        //    if (dbRoute == null)
        //    {
        //        throw new ArgumentException("DBRoute不能为空！");
        //    }
        //    RWTableInfo si = RWTableInfo.ParseRWTableInfo(sql);
        //    string routeKey;
        //    if (dbRoute.HasEmptyTableRouteKey())
        //    {
        //        routeKey = dbRoute.RouteKey;
        //    }

        //    else if (si.MainTable != null)
        //    {
        //        routeKey = dbRoute.GetTableRouteKey(si.MainTable);
        //    }
        //    else
        //    {
        //        routeKey = dbRoute.RouteKey;
        //    }
        //     DBShardingRuntime dbShardingRuntime = DBShardingRuntime.get();
        //     string[] tableNames = si.AllTables.ToArray();
        //    DelegateConnection con;
        //    if (AbstractDBImpl.thQueryAlone.Value.Get())
        //    {
        //        //con = (DelegateConnection)TX.__getAloneConnection(routeKey, !si.isWritedSQL(), si.getMainTable(), tableNames);
        //    }
        //    else
        //    {
        //        con = (DelegateConnection)TX.__getConnection(routeKey, !si.isWritedSQL(), si.getMainTable(), tableNames);
        //    }
        //}

    }
}
