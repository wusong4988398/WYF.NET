using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WYF.Common;
using WYF.KSQL;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Formater;
using WYF.KSQL.Parser;

namespace WYF.DbEngine
{
    internal class SqlDatabase : AbstractDatabase
    {

        private static string _alwaysOnString;


        private static void AppendtBatchExecuteSql(StringBuilder sql, BatchSqlParam param, string where, string createTempTableSql, string tempTabAliases)
        {
            int index = createTempTableSql.IndexOf("table");
            string str = createTempTableSql.Substring(index + 5, (createTempTableSql.IndexOf("(") - index) - 5);
            param.data.TableName = str.Trim();
            string batchParamJoinSql = GetBatchParamJoinSql(param, tempTabAliases);
            foreach (string str3 in param.GetJoinExpression())
            {
                sql.Append(str3);
                sql.Append(" ");
            }
            sql.AppendFormat(" inner join {0} {1} on {2} ", str.Trim(), tempTabAliases, batchParamJoinSql);
            if (!string.IsNullOrWhiteSpace(where))
            {
                sql.AppendFormat(" where {0} ", where);
            }
        }

        protected virtual void ArrayToDataTable(int itype, DbParameter param, string udttypename = "")
        {
            string[] strArray;
            if (string.IsNullOrEmpty(udttypename))
            {
                param.ParameterName = param.ParameterName + "_udt" + itype.ToString();
            }
            if (param.Value.ToString().Contains("^"))
            {
                strArray = ((string)param.Value).Split(new string[] { "^" }, StringSplitOptions.None);
            }
            else if (param.Value is string)
            {
                strArray = ((string)param.Value).Split(new char[] { ',' });
            }
            else
            {
                strArray = (string[])param.Value;
            }
            this.GeneralUdtParamValue(itype, (SqlParameter)param, strArray);
        }

        public override int BatchDelete(BatchSqlParam param, string where = "")
        {
            string tempTabAliases = "tmp_b";
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" delete {0} from {0} {1} ", param.TableName, param.TableAliases);
            return this.DoBatchExecute(param, where, sql, tempTabAliases);
        }

        public override IDataReader BatchSelect(BatchSqlParam param, string selectFields, string where = "")
        {
            string tempTabAliases = "tmp_b";
            StringBuilder sql = new StringBuilder();
            string str2 = selectFields.Replace("@", "tmp_b.");
            sql.AppendFormat(" select {0} ", str2);
            sql.AppendLine();
            sql.AppendFormat("from {0} {1} ", param.TableName, param.TableAliases);
            return this.DoBatchExecuteReader(param, where, sql, tempTabAliases);
        }

        public override int BatchUpdate(BatchSqlParam param, string where = "")
        {
            string tempTabAliases = "tmp_b";
            StringBuilder sql = new StringBuilder();
            string batchParamSetSql = GetBatchParamSetSql(param, tempTabAliases);
            sql.AppendFormat(" update {0} set {1} from {0} {2} ", param.TableName, batchParamSetSql, param.TableAliases);
            return this.DoBatchExecute(param, where, sql, tempTabAliases);
        }

        public override void BulkCopy(DbCommand cmd, DataTable dt, bool UseTransaction = false)
        {
            this.BulkInserts(cmd, dt);
        }

        public override void BulkInserts(DbCommand cmd, DataTable dt)
        {
            using (ConnectionWrapper wrapper = base.GetOpenConnection())
            {
                AbstractDatabase.PrepareCommand(cmd, wrapper.Connection);
                this.CopyDataToDestination(cmd, dt);
            }
        }

        public void BulkInsertsSupportBinary(DbCommand cmd, DataTable dt)
        {
            using (ConnectionWrapper wrapper = base.GetOpenConnection())
            {
                AbstractDatabase.PrepareCommand(cmd, wrapper.Connection);
                this.CopyDataToDestination(cmd, dt);
            }
        }

        public override void ConvertTableFun(DbCommand command)
        {
            if (command.Parameters.Count != 0)
            {
                string commandText = command.CommandText;
                int count = command.Parameters.Count;
                for (int i = 0; i < count; i++)
                {
                    DbParameter param = command.Parameters[i];
                    string parameterName = param.ParameterName;
                    if (commandText.Contains(parameterName + "_udt1"))
                    {
                        this.ArrayToDataTable(1, param, "");
                    }
                    if (commandText.Contains(parameterName + "_udt2"))
                    {
                        this.ArrayToDataTable(2, param, "");
                    }
                    if (commandText.Contains(parameterName + "_udt3"))
                    {
                        this.ArrayToDataTable(3, param, "");
                    }
                }
            }
        }

        private void CopyDataToDestination(DbCommand cmd, DataTable table)
        {
            SqlConnection connection = cmd.Connection as SqlConnection;
            //using (SqlTransaction transaction = connection.BeginTransaction())
            //{
            //using (SqlBulkCopy copy = new SqlBulkCopy(connection,SqlBulkCopyOptions.CheckConstraints,transaction))
            using (SqlBulkCopy copy = new SqlBulkCopy(connection))
            {
                copy.BatchSize = 0x1388;
                copy.BulkCopyTimeout = 60;
                copy.ColumnMappings.Clear();
                foreach (DataColumn column in table.Columns)
                {
                    copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                copy.DestinationTableName = table.TableName;
                try
                {
                    copy.WriteToServer(table);
                    copy.Close();
                    //transaction.Commit();
                }
                catch (SqlException exception)
                {
                    //transaction.Rollback();
                    throw exception;
                }
            }
            // }
        }

        private string CreateBatchExecuteTempTableSQL(BatchSqlParam param)
        {
            if (!param.GetCreateTempTableSQL().IsNullOrWhiteSpace())
            {
                return param.GetCreateTempTableSQL();
            }
            StringBuilder builder = new StringBuilder();
            Dictionary<string, string> dictionary = param.ColumnTypeExprOfSetField();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                builder.AppendFormat("{0} {1}, ", pair.Key, pair.Value);
            }
            foreach (KeyValuePair<string, string> pair2 in param.ColumnTypeExprOfWhereField())
            {
                if (!dictionary.ContainsKey(pair2.Key))
                {
                    builder.AppendFormat("{0} {1}, ", pair2.Key, pair2.Value);
                }
            }
            string str = "tmp" + SequentialGuid.NewNativeGuid().ToString("N").Substring(0, 0x1b);
            StringBuilder builder2 = new StringBuilder();
            builder2.AppendFormat(" create table #{0} ({1})", str, builder.Remove(builder.Length - 2, 2));
            return builder2.ToString();
        }

        protected override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        protected override DbConnection CreateConnection()
        {
            if ((ReadOnlyDBScope.Current != null) && (Transaction.Current == null))
            {
                return new SqlConnection(string.Format("{0}{1}", base.ConnectionString, AlwaysOnString));
            }
            return new SqlConnection(base.ConnectionString);
        }

        protected override DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public override string CreateSessionTempTable(string createSql, string isolation = "")
        {
            string strtmp = string.Format("{0}{1}", isolation, createSql);
            string str2 = string.Format("#{0}", strtmp.GetMD5String());
            string commandText = string.Format("Create Table {0}{1}", str2, createSql);
            using (DbCommand command = base.CreateCommandByCommandType(CommandType.Text, commandText, 0))
            {
                base.ExecuteNonQuery(command, null);
            }
            return str2;
        }

        private int DoBatchExecute(BatchSqlParam param, string where, StringBuilder sql, string tempTabAliases)
        {
            int num;
            string createTempTableSql = this.CreateBatchExecuteTempTableSQL(param);
            AppendtBatchExecuteSql(sql, param, where, createTempTableSql, tempTabAliases);
            using (DbCommand command = this.CreateCommand())
            {
                using (ConnectionWrapper wrapper = base.GetOpenConnection())
                {
                    AbstractDatabase.PrepareCommand(command, wrapper.Connection);
                    command.CommandText = createTempTableSql;
                    command.ExecuteNonQuery();
                    this.CopyDataToDestination(command, param.data);
                    command.CommandText = sql.ToString();
                    num = command.ExecuteNonQuery();
                }
            }
            return num;
        }

        private IDataReader DoBatchExecuteReader(BatchSqlParam param, string where, StringBuilder sql, string tempTabAliases)
        {
            IDataReader reader;
            string createTempTableSql = this.CreateBatchExecuteTempTableSQL(param);
            AppendtBatchExecuteSql(sql, param, where, createTempTableSql, tempTabAliases);
            ConnectionWrapper openConnection = base.GetOpenConnection();
            try
            {
                using (DbCommand command = this.CreateCommand())
                {
                    AbstractDatabase.PrepareCommand(command, openConnection.Connection);
                    command.CommandText = createTempTableSql;
                    command.ExecuteNonQuery();
                    this.CopyDataToDestination(command, param.data);
                    command.CommandText = sql.ToString();
                    if ((Transaction.Current == null) && (SessionScope.Current == null))
                    {
                        return command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.KeyInfo);
                    }
                    reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                }
            }
            catch (Exception exception)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("DoBatchExecuteReader", sql.ToString(), exception.Message, exception);
            }
            return reader;
        }

        public override int ExecuteBatch(List<string> sqlArray, int batchSize, int commandTimeout = 30)
        {
            int num4;
            int num = 0;
            if (sqlArray.Count == 0)
            {
                return num;
            }
            StringBuilder builder = new StringBuilder();
            try
            {
                using (DbCommand command = this.CreateCommand())
                {
                    command.CommandTimeout = AbstractDatabase.GetCommandTimeOut(commandTimeout);
                    using (ConnectionWrapper wrapper = base.GetOpenConnection())
                    {
                        AbstractDatabase.PrepareCommand(command, wrapper.Connection);
                        int num2 = 0;
                        int count = sqlArray.Count;
                        builder = new StringBuilder();
                        foreach (string str in sqlArray)
                        {
                            num2++;
                            builder.AppendLine(str);
                            if ((num2 == count) || ((num2 % batchSize) == 0))
                            {
                                command.CommandText = TransUtil.Translate(builder.ToString(), base.DbType);
                                num = command.ExecuteNonQuery();
                                builder.Clear();
                            }
                        }
                        num4 = num;
                    }
                }
            }
            catch (DbException exception)
            {
                //Logger.Error("ExecuteBatch error:", ObjectUtils.Object2String(exception.Message) + " " + builder.ToString(), exception);
                throw new ExceptionDatabase("ExecuteBatch", builder.ToString(), exception.Message, exception);
            }
            return num4;
        }

        public override int ExecuteBatchParallel(List<string> sqlArray, int batchSize, int commandTimeout = 0)
        {
            int num;
            int returnCount = 0;
            if (sqlArray.Count == 0)
            {
                return returnCount;
            }
            StringBuilder sb = new StringBuilder();
            try
            {
                using (DbCommand command = this.CreateCommand())
                {
                    using (ConnectionWrapper wrapper = base.GetOpenConnection())
                    {
                        AbstractDatabase.PrepareCommand(command, wrapper.Connection);
                        int size = 0;
                        int count = sqlArray.Count;
                        sb = new StringBuilder();
                        Parallel.ForEach<string>(sqlArray, delegate (string sql) {
                            size++;
                            sb.AppendLine(sql);
                            if ((size == count) || ((size % batchSize) == 0))
                            {
                                command.CommandText = TransUtil.Translate(sb.ToString(), this.DbType);
                                returnCount = command.ExecuteNonQuery();
                                sb.Clear();
                            }
                        });
                        num = returnCount;
                    }
                }
            }
            catch (DbException exception)
            {
                //Logger.Error("ExecuteBatch error:", ObjectUtils.Object2String(exception.Message) + " " + sb.ToString(), exception);
                throw new ExceptionDatabase("ExecuteBatch", sb.ToString(), exception.Message, exception);
            }
            return num;
        }



        private void GeneralUdtParamValue(int itype, SqlParameter param, string[] sarrayvalue)
        {
            Func<string, SqlDataRecord> selector = null;
            Func<string, SqlDataRecord> func2 = null;
            SqlMetaData md = null;
            param.SqlDbType = SqlDbType.Udt;
            if (itype == 1)
            {
                md = new SqlMetaData("FID", SqlDbType.BigInt);
                param.TypeName = "udt_inttable";
            }
            if (itype == 2)
            {
                md = new SqlMetaData("FID", SqlDbType.VarChar, 450L);
                param.TypeName = "udt_varchartable";
            }
            if (itype == 3)
            {
                md = new SqlMetaData("FID", SqlDbType.NVarChar, 450L);
                param.TypeName = "udt_nvarchartable";
            }
            if ((itype != 2) && (itype != 3))
            {
                if (func2 == null)
                {
                    func2 = delegate (string x) {
                        SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { md });
                        record.SetInt64(0, long.Parse(x));
                        return record;
                    };
                }
                IEnumerable<SqlDataRecord> enumerable2 = sarrayvalue.Select<string, SqlDataRecord>(func2);
                param.Value = enumerable2;
            }
            else
            {
                if (selector == null)
                {
                    selector = delegate (string x) {
                        SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { md });
                        record.SetString(0, x);
                        return record;
                    };
                }
                IEnumerable<SqlDataRecord> enumerable = sarrayvalue.Select<string, SqlDataRecord>(selector);
                param.Value = enumerable;
            }
            param.SqlDbType = SqlDbType.Structured;
        }

        private static string GetBatchParamJoinSql(BatchSqlParam param, string tempTabAliases)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in param.GetWhereField(tempTabAliases))
            {
                builder.AppendFormat(" {0} and", pair.Value);
            }
            string str = builder.ToString();
            return str.Substring(0, str.Length - 3);
        }

        private static string GetBatchParamSetSql(BatchSqlParam param, string tempTabAliases)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in param.GetSetSqlForSQLServer(tempTabAliases))
            {
                SqlExpr expr = new SqlExprParser(pair.Value).expr();
                SQLFormater formater = new MSTransactSQLFormater();
                formater.FormatExpr(expr);
                string buffer = formater.GetBuffer();
                builder.AppendFormat(" {0} = {1},", pair.Key, buffer);
            }
            string str2 = builder.ToString();
            return str2.Substring(0, str2.Length - 1);
        }

        protected override DbParameter GetParameter(DbConnection conn, string name, KDbType custemDbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = name
            };
            if (custemDbType == KDbType.RefCursor)
            {
                param.DbType = System.Data.DbType.Object;
            }
            else if (custemDbType == KDbType.udt_inttable)
            {
                param.DbType = System.Data.DbType.String;
                param.SqlDbType = SqlDbType.Udt;
                param.ParameterName = param.ParameterName + "_udt1";
                param.TypeName = "udt_inttable";
                string[] sarrayvalue = (from x in ((IEnumerable)value).Cast<object>() select x.ToString()).ToArray<string>();
                this.GeneralUdtParamValue(1, param, sarrayvalue);
            }
            else if (custemDbType == KDbType.udt_varchartable)
            {
                param.DbType = System.Data.DbType.String;
                param.SqlDbType = SqlDbType.Udt;
                param.ParameterName = param.ParameterName + "_udt2";
                param.TypeName = "udt_varchartable";
                string[] strArray2 = (from x in ((IEnumerable)value).Cast<object>() select x.ToString()).ToArray<string>();
                this.GeneralUdtParamValue(2, param, strArray2);
            }
            else if (custemDbType == KDbType.udt_nvarchartable)
            {
                param.DbType = System.Data.DbType.String;
                param.SqlDbType = SqlDbType.Udt;
                param.ParameterName = param.ParameterName + "_udt3";
                param.TypeName = "udt_nvarchartable";
                string[] strArray3 = (from x in ((IEnumerable)value).Cast<object>() select x.ToString()).ToArray<string>();
                this.GeneralUdtParamValue(3, param, strArray3);
            }
            else
            {


                param.DbType = (DbType)custemDbType;
                param.Value = value ?? DBNull.Value;
            }
            param.Size = size;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        [Obsolete("该方法即将废弃")]
        protected override DbParameter GetParameter(DbConnection conn, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value, string udttypename = "")
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                DbType = dbType,
                Size = size,
                Value = value ?? DBNull.Value,
                Direction = direction,
                IsNullable = nullable,
                SourceColumn = sourceColumn,
                SourceVersion = sourceVersion
            };
            if (!string.IsNullOrEmpty(udttypename))
            {
                if (udttypename.Equals("udt_inttable", StringComparison.OrdinalIgnoreCase))
                {
                    this.ArrayToDataTable(1, param, udttypename);
                }
                if (udttypename.Equals("udt_varchartable", StringComparison.OrdinalIgnoreCase))
                {
                    this.ArrayToDataTable(2, param, udttypename);
                }
                if (udttypename.Equals("udt_nvarchartable", StringComparison.OrdinalIgnoreCase))
                {
                    this.ArrayToDataTable(3, param, udttypename);
                }
            }
            return param;
        }

        protected override int NonQueryByArray(DbCommand command, IEnumerable<SqlParam> listParam)
        {
            return command.ExecuteNonQuery();
        }

        public override bool TestDbOpenConnection(string strDbConnectionString, out string strErrMsg)
        {
            strErrMsg = string.Empty;
            bool flag = true;
            SqlConnection connection = new SqlConnection(strDbConnectionString);
            try
            {
                connection.Open();
                flag = true;
            }
            catch (SqlException exception)
            {
                flag = false;
                strErrMsg = string.Format("连接数据库失败！\n{0}", exception.Message);
            }
            catch (Exception exception2)
            {
                flag = false;
                strErrMsg = string.Format("连接数据库失败！\n{0}", exception2.Message);
            }
            finally
            {
                connection.Close();
            }
            return flag;
        }

        private static string TransExpression(string exprString)
        {
            SqlExpr expr = new SqlExprParser(exprString).expr();
            SQLFormater formater = new MSTransactSQLFormater();
            formater.FormatExpr(expr);
            return formater.GetBuffer();
        }

        private static string AlwaysOnString
        {
            get
            {
                if (_alwaysOnString == null)
                {
                    string str = ConfigurationManager.AppSettings["AlwaysOn"];
                    _alwaysOnString = "";
                    if (!string.IsNullOrEmpty(str) && Convert.ToBoolean(str))
                    {
                        _alwaysOnString = "ApplicationIntent=ReadOnly;";
                    }
                }
                return _alwaysOnString;
            }
        }
    }
}
