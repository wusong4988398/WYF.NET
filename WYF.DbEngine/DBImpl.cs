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

        public override QueryResult<T> Query<T>(DBRoute dbRoute, IDbConnection con, bool close, string sql, Func<IDataReader, T> action, bool convert, params object[] paramVarArgs)
        {
            bool rollback = false;
            QueryResource resource = new QueryResource();
            IDbCommand dbcommand = con.CreateCommand();
            dbcommand.CommandText = sql;
            dbcommand.CommandType = CommandType.Text;
            IDataReader rs = dbcommand.ExecuteReader();
            T result = action.Invoke(rs);
            QueryResult<T> queryResult = new QueryResult<T>(result, resource);
            return queryResult;
        }
    }
}
