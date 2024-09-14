using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine
{
    public interface IDatabase
    {

        int BatchDelete(BatchSqlParam param, string where = "");
        IDataReader BatchSelect(BatchSqlParam param, string selectFields, string where = "");
        int BatchUpdate(BatchSqlParam param, string where = "");
        void BulkCopy(DbCommand cmd, DataTable dt, bool UseTransaction = false);
        void BulkInserts(DbCommand cmd, DataTable dt);
        void ConvertTableFun(DbCommand command);
        DbCommand CreateCommandByCommandType(CommandType cmdtype, string strSQL, int commandTimeout = 0);
        DbCommand CreateCommandByCommandType(CommandType commandType, string strSQL, bool needTranslate, int commandTimeout = 0);
        string CreateSessionTempTable(string createSql, string isolation = "");
        int ExecuteBatch(List<string> sqlArray, int batchSize, int commandTimeout = 30);
        int ExecuteBatchParallel(List<string> sqlArray, int batchSize, int commandTimeout = 30);
        DataSet ExecuteDataSet(List<string> sqlArray, string[] tableNames);
        DataSet ExecuteDataSet(DbCommand cmd, IEnumerable<SqlParam> listParam = null);
        DataSet ExecuteDataSet(DbCommand command, DataSet dataSet, string tableName, IEnumerable<SqlParam> listParam = null);
        void ExecuteMerge(DbCommand cmd, IEnumerable<SqlParam> listParam = null);
        int ExecuteNonQuery(DbCommand cmd, IEnumerable<SqlParam> listParam = null);
        int ExecuteNonQueryWithNewCn(DbCommand command, IEnumerable<SqlParam> listParam = null, bool bNewCn = true);
        IDataReader ExecuteReader(DbCommand cmd, IEnumerable<SqlParam> paramList);
        IDataReader ExecuteReader(DbCommand cmd, IEnumerable<SqlParam> paramList, CommandBehavior cmdBehavior);
        IDataReader ExecuteReaderWithNewCn(DbCommand cmd, IEnumerable<SqlParam> paramList);
        IDataReader ExecuteReaderWithNewCn(DbCommand cmd, IEnumerable<SqlParam> paramList, CommandBehavior cmdBehavior);
        object ExecuteScalar(DbCommand command, params SqlParam[] paramList);
        List<SqlParam> ExecuteStoreProcedure(DbCommand cmd, IEnumerable<SqlParam> lstParam = null);
        DbConnection GetNewOpenConnection();
        DbConnection GetWrapConnection();
        bool TestDbOpenConnection(string strDbConnectionString, out string strErrMsg);
        string ConnectionString { get; set; }
    }
}
