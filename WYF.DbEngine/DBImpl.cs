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
    }
}
