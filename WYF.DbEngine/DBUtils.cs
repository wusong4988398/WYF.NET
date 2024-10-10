using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine;

namespace WYF.DbEngine
{
    public static class DBUtils
    {
        public static IDataReader ExecuteReader(Context ctx, string strSQL, IEnumerable<SqlParam> paramList, CommandType cmdtype, CommandBehavior cmdBehavior, bool bNewCn)
        {
            IDatabase database = DatabaseFactory.CreateDataBase(ctx);
            using (DbCommand command = database.CreateCommandByCommandType(cmdtype, strSQL, 0))
            {
                if (bNewCn)
                {
                    return database.ExecuteReaderWithNewCn(command, paramList, cmdBehavior);
                }
                return database.ExecuteReader(command, paramList, cmdBehavior);
            }
        }

        public static IDataReader ExecuteReader(Context ctx, string strSQL, List<SqlParam> paramList)
        {
            return ExecuteReader(ctx, strSQL, paramList, CommandType.Text, false);
        }
        public static IDataReader ExecuteReader(Context ctx, string strSQL, IEnumerable<SqlParam> paramList, CommandType cmdtype, bool bNewCn)
        {
            return ExecuteReader(ctx, strSQL, paramList, cmdtype, CommandBehavior.KeyInfo, bNewCn);
        }
        public static T ExecuteScalar<T>(Context ctx, string strSql, T defaultValue, params SqlParam[] paramList)
        {

            IDatabase database = DatabaseFactory.CreateDataBase(ctx);
            using (DbCommand command = database.CreateCommandByCommandType(CommandType.Text, strSql, 0))
            {
                return DBReaderUtils.ConvertTo<T>(database.ExecuteScalar(command, paramList), null, defaultValue);
            }
        }

        public static DataSet ExecuteDataSet(Context ctx, CommandType commandType, string strSQL, List<SqlParam> paramList)
        {
            IDatabase database = DatabaseFactory.CreateDataBase(ctx);
            using (DbCommand command = database.CreateCommandByCommandType(commandType, strSQL, 0))
            {
                return database.ExecuteDataSet(command, paramList);
            }
        }

        public static DataSet ExecuteDataSet(Context ctx, string strSQL, DataSet ds, string tableName, List<SqlParam> paramList)
        {
            IDatabase database = DatabaseFactory.CreateDataBase(ctx);
            using (DbCommand command = database.CreateCommandByCommandType(CommandType.Text, strSQL, 0))
            {
                return database.ExecuteDataSet(command, ds, tableName, paramList);
            }
        }

        public static int Execute(Context ctx, string strSQL, IEnumerable<SqlParam> paramList, bool needTranslate)
        {
            IDatabase database = DatabaseFactory.CreateDataBase(ctx);
            using (DbCommand command = database.CreateCommandByCommandType(CommandType.Text, strSQL, needTranslate, 0))
            {
                return database.ExecuteNonQuery(command, paramList);
            }
        }

    
    }
}
