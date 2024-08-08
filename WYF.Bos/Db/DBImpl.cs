using WYF.Bos.algo;
using WYF.Bos.db.tx;
using WYF.Bos.trace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public  class DBImpl: AbstractDBImpl
    {
        public override int[] ExecuteBatch(DBRoute dbRoute, String sql, List<Object[]> paramsList, TraceSpan ts)
        {

            // return  new int[] { SqlsugarHelper.Instance.Ado.ExecuteCommand(sql, paramsList) };
            return new int[] { 1 };
        }

  

        public override QueryResult<IDataSet> Query(DBRoute dbRoute, DelegateConnection con, bool close, string sql, Func<object, object> callback, bool convert, params object[] parameters)
        {
            throw new NotImplementedException();
        }
        //public override QueryResult<T> Query<T>(DBRoute dbRoute, SqlSugarProvider con, bool close, string sql, Func<IDataReader, T> action, bool convert, params object[] parameters)
        public override QueryResult<T> Query<T>(DBRoute dbRoute, IDbConnection con, bool close, string sql, Func<IDataReader,T> action, bool convert, params object[] parameters)
        {
            bool rollback = false;
           
            QueryResource resource = new QueryResource();
            IDbCommand dbcommand= con.CreateCommand();
            dbcommand.CommandText = sql;
            dbcommand.CommandType = CommandType.Text;
            

            IDataReader rs = dbcommand.ExecuteReader();
            T result  = action.Invoke(rs);
            QueryResult<T> queryResult=  new QueryResult<T>(result, resource);
            return queryResult;
        }
    }
}
