using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WYF.Common;
using WYF.KSQL;

namespace WYF.DbEngine
{
    public abstract class AbstractDatabase : IDatabase
    {

        private static int _defaultCommandTimeOut = -1;
        private string connectionString;


        public AbstractDatabase()
        {
        }

        public abstract int BatchDelete(BatchSqlParam param, string where = "");
        public abstract IDataReader BatchSelect(BatchSqlParam param, string selectFields, string where = "");
        public abstract int BatchUpdate(BatchSqlParam param, string where = "");
        public virtual void BulkCopy(DbCommand cmd, DataTable dt, bool UseTransaction = false)
        {
        }

        public virtual void BulkInserts(DbCommand cmd, DataTable dt)
        {
        }

        public virtual void BulkInsertsSupportBinary(DbCommand cmd, DataTable dt)
        {
        }

        public virtual void ConvertTableFun(DbCommand command)
        {
        }

        protected abstract DbCommand CreateCommand();
        public DbCommand CreateCommandByCommandType(CommandType commandType, string commandText, int commandTimeout = 0)
        {
            return this.CreateCommandByCommandType(commandType, commandText, true, 0);
        }

        public DbCommand CreateCommandByCommandType(CommandType commandType, string commandText, bool needTranslate, int commandTimeout = 0)
        {
            DbCommand command = this.CreateCommand();
            command.CommandType = commandType;
            if ((command is OracleCommand) && (command.CommandType == CommandType.StoredProcedure))
            {
                ((OracleCommand)command).BindByName = false;
            }
            command.CommandTimeout = GetCommandTimeOut(commandTimeout);
            command.CommandText = needTranslate ? TransUtil.Translate(commandText, this.DbType) : commandText;
            return command;
        }

        protected abstract DbConnection CreateConnection();
        protected abstract DbDataAdapter CreateDataAdapter();
        public abstract string CreateSessionTempTable(string createSql, string isolation = "");
        private IDataReader DoExecuteReader(DbCommand command, CommandBehavior cmdBehavior)
        {
       
            IDataReader reader = command.ExecuteReader(cmdBehavior);
    
            if ((reader is OracleDataReader) && (((OracleDataReader)reader).RowSize > 0L))
            {
                ((OracleDataReader)reader).FetchSize = ((OracleDataReader)reader).RowSize * 0x7d0L;
            }
            return reader;
        }

        private object DoExecuteScalar(DbCommand command)
        {
   
            object obj2 = command.ExecuteScalar();
   
            return obj2;
        }

        private void DoLoadDataSet(IDbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (tableNames == null || !tableNames.Any(n => !string.IsNullOrEmpty(n)))
            {
                throw new ArgumentException("tableNames must contain at least one non-null or non-empty string.");
            }

            try
            {
      

                using (var adapter = GetDataAdapter())
                {
                    ((IDbDataAdapter)adapter).SelectCommand = command;

                    // Map table names to the dataset
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        adapter.TableMappings.Add($"Table{i}", tableNames[i]);
                    }

                    adapter.Fill(dataSet);
                }
            }
            catch (SqlException ex)
            {
                throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, ex.Message);
            }
            catch (OracleException ex)
            {
                throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, ex.Message);
            }
            catch (NpgsqlException ex)
            {
                throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, ex.Message);
            }
            catch (MySqlException ex)
            {
                throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, ex.Message);
            }
            finally
            {
                ReleaseParam(command);
            }
        }

        //private void DoLoadDataSet(IDbCommand command, DataSet dataSet, string[] tableNames)
        //{
        //    if ((tableNames == null) || (tableNames.Length == 0))
        //    {
        //        throw new ArgumentNullException("tableNames");
        //    }
        //    int index = 0;
        //    while (index < tableNames.Length)
        //    {
        //        if (string.IsNullOrEmpty(tableNames[index]))
        //        {
        //            goto Label_013F;
        //        }
        //        index++;
        //    }
        //    try
        //    {
        //        try
        //        {
        //            SqlConSumConterInfo instance = null;
        //            if (!command.CommandText.ToUpper().Contains("DBCC"))
        //            {
        //                instance = SqlConSumConterInfo.GetInstance(this.curContext);
        //            }
        //            using (DbDataAdapter adapter = this.GetDataAdapter())
        //            {
        //                ((IDbDataAdapter)adapter).SelectCommand = command;
        //                DateTime now = DateTime.Now;
        //                string str = "Table";
        //                for (int i = 0; i < tableNames.Length; i++)
        //                {
        //                    string sourceTable = (i == 0) ? str : (str + i);
        //                    adapter.TableMappings.Add(sourceTable, tableNames[i]);
        //                }
        //                adapter.Fill(dataSet);
        //            }
        //            if (instance != null)
        //            {
        //                instance.EndCount(command.CommandText, this.curContext);
        //            }
        //        }
        //        catch (SqlException exception)
        //        {
        //            throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, exception.Message);
        //        }
        //        catch (OracleException exception2)
        //        {
        //            throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, exception2.Message);
        //        }
        //        catch (NpgsqlException exception3)
        //        {
        //            throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, exception3.Message);
        //        }
        //        catch (MySqlException exception4)
        //        {
        //            throw new ExceptionDatabase("DoLoadDataSet", command.CommandText, exception4.Message);
        //        }
        //        return;
        //    }
        //    finally
        //    {
        //        this.ReleaseParam(command);
        //    }
        //    Label_013F:
        //    throw new ArgumentException("tableNames[" + index + "]");
        //}

        public abstract int ExecuteBatch(List<string> sqlArray, int batchSize, int commandTimeout = 30);
        public abstract int ExecuteBatchParallel(List<string> sqlArray, int batchSize, int commandTimeout = 0);
        public DataSet ExecuteDataSet(List<string> sqlArray, string[] tableNames)
        {
            using (DataSet set = new DataSet())
            {
                set.Locale = CultureInfo.InvariantCulture;
                using (ConnectionWrapper wrapper = this.GetOpenConnection())
                {
                    using (DbCommand command = this.CreateCommand())
                    {
                        PrepareCommand(command, wrapper.Connection);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = GetCommandTimeOut(_defaultCommandTimeOut);
                        using (DbDataAdapter adapter = this.GetDataAdapter())
                        {
                            int index = 0;
                            foreach (string str in sqlArray)
                            {
                                using (DataSet set2 = new DataSet())
                                {
                                    set2.Locale = CultureInfo.InvariantCulture;
                                    command.CommandText = TransUtil.Translate(str, this.DbType);
                                    ((IDbDataAdapter)adapter).SelectCommand = command;
                                    adapter.Fill(set2);
                                    set2.Tables[0].TableName = tableNames[index];
                                    set.Tables.Add(set2.Tables[0].Copy());
                                    index++;
                                }
                            }
                        }
                        this.ReleaseParam(command);
                    }
                }
                return set;
            }
        }

        public DataSet ExecuteDataSet(DbCommand command, IEnumerable<SqlParam> listParam = null)
        {
            using (DataSet set = new DataSet())
            {
                set.Locale = CultureInfo.InvariantCulture;
                this.LoadDataSet(command, set, "Table", listParam);
                return set;
            }
        }

        public DataSet ExecuteDataSet(DbCommand command, DataSet dataSet, string tableName, IEnumerable<SqlParam> listParam = null)
        {
            dataSet.Locale = CultureInfo.InvariantCulture;
            this.LoadDataSet(command, dataSet, tableName, listParam);
            return dataSet;
        }
        public void ExecuteMerge(DbCommand cmd, IEnumerable<SqlParam> listParam = null)
        {
            try
            {
                using (ConnectionWrapper wrapper = this.GetOpenConnection())
                {
                    PrepareCommand(cmd, wrapper.Connection);
                    this.PrepareParameter(wrapper.Connection, cmd, listParam);
                    cmd.ExecuteNonQuery();
              
                }
            }
            catch (SqlException exception)
            {
                throw new ExceptionDatabase("ExecuteMerge", cmd.CommandText, exception.Message, exception);
            }
            catch (OracleException exception2)
            {
                throw new ExceptionDatabase("ExecuteMerge", cmd.CommandText, exception2.Message, exception2);
            }
            catch (NpgsqlException exception3)
            {
                throw new ExceptionDatabase("ExecuteMerge", cmd.CommandText, exception3.Message, exception3);
            }
            catch (MySqlException exception4)
            {
                throw new ExceptionDatabase("ExecuteMerge", cmd.CommandText, exception4.Message, exception4);
            }
            finally
            {
                this.ReleaseParam(cmd);
            }
        }

        public int ExecuteNonQuery(DbCommand command, IEnumerable<SqlParam> listParam = null)
        {
            int num2;
            try
            {
                int num = 0;
                using (ConnectionWrapper wrapper = this.GetOpenConnection())
                {
                    PrepareCommand(command, wrapper.Connection);
                    this.PrepareParameter(wrapper.Connection, command, listParam);
                    num = command.ExecuteNonQuery();
                }
                num2 = num;
            }
            catch (SqlException exception)
            {
                throw new ExceptionDatabase("BOS_ExecuteNonQuery", command.CommandText, exception.Message, exception);
            }
            catch (OracleException exception2)
            {
                throw new ExceptionDatabase("BOS_ExecuteNonQuery", command.CommandText, exception2.Message, exception2);
            }
            catch (NpgsqlException exception3)
            {
                throw new ExceptionDatabase("BOS_ExecuteNonQuery", command.CommandText, exception3.Message, exception3);
            }
            catch (MySqlException exception4)
            {
                throw new ExceptionDatabase("BOS_ExecuteNonQuery", command.CommandText, exception4.Message, exception4);
            }
            finally
            {
                this.ReleaseParam(command);
            }
            return num2;
        }

        public int ExecuteNonQueryByArray(DbCommand command, IEnumerable<SqlParam> listParam)
        {
            if ((listParam == null) || (listParam.Count<SqlParam>() == 0))
            {
                return this.ExecuteNonQuery(command, null);
            }
            using (ConnectionWrapper wrapper = this.GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                this.PrepareParameter(wrapper.Connection, command, listParam);
                return this.NonQueryByArray(command, listParam);
            }
        }

        public int ExecuteNonQueryWithNewCn(DbCommand command, IEnumerable<SqlParam> listParam = null, bool bNewCn = true)
        {
            int num2;
            try
            {
                int num = 0;
                ConnectionWrapper openConnection = null;
                if (bNewCn)
                {
                    openConnection = new ConnectionWrapper(this.GetNewOpenConnection(), true);
                }
                else
                {
                    openConnection = this.GetOpenConnection(false);
                }
                using (openConnection)
                {
                    PrepareCommand(command, openConnection.Connection);
                    this.PrepareParameter(openConnection.Connection, command, listParam);
                    num = command.ExecuteNonQuery();
                    num2 = num;
                }
            }
            catch (SqlException exception)
            {
                throw new ExceptionDatabase("BOS_ExecuteNonQueryWithNewCn", command.CommandText, exception.Message);
            }
            catch (OracleException exception2)
            {
                throw new ExceptionDatabase("BOS_ExecuteNonQueryWithNewCn", command.CommandText, exception2.Message);
            }
            finally
            {
                this.ReleaseParam(command);
            }
            return num2;
        }

        public IDataReader ExecuteReader(DbCommand command, IEnumerable<SqlParam> paramList)
        {
            return this.ExecuteReader(command, paramList, CommandBehavior.KeyInfo, false);
        }
        public IDataReader ExecuteReader(DbCommand command, object[] parameters)
        {
            return this.ExecuteReader(command, parameters, CommandBehavior.KeyInfo, false);
        }

        public IDataReader ExecuteReader(DbCommand command, IEnumerable<SqlParam> paramList, CommandBehavior cmdBehavior)
        {
            return this.ExecuteReader(command, paramList, cmdBehavior, false);
        }


        public IDataReader ExecuteReader(DbCommand command, object[] parameters, CommandBehavior cmdBehavior)
        {
            return this.ExecuteReader(command, parameters, cmdBehavior, false);
        }
        //object[] parameters
        private IDataReader ExecuteReader(DbCommand command, object[] parameters, CommandBehavior cmdBehavior, bool bNewCn = false)
        {
            ConnectionWrapper openConnection = null;
            IDataReader reader2;
            if (bNewCn)
            {
                openConnection = new ConnectionWrapper(this.GetNewOpenConnection(), true);
            }
            else
            {
                openConnection = this.GetOpenConnection(false);
            }
            try
            {
                PrepareCommand(command, openConnection.Connection);
                this.PrepareParameter2(openConnection.Connection, command, parameters);
                if (bNewCn)
                {
                    return this.DoExecuteReader(command, CommandBehavior.CloseConnection | cmdBehavior);
                }
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    return this.DoExecuteReader(command, CommandBehavior.CloseConnection | cmdBehavior);
                }
                reader2 = this.DoExecuteReader(command, cmdBehavior);
            }
            catch (SqlException exception)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception.Message, exception);
            }
            catch (OracleException exception2)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception2.Message, exception2);
            }
            catch (NpgsqlException exception3)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception3.Message, exception3);
            }
            catch (MySqlException exception4)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception4.Message, exception4);
            }
            catch
            {
                if (((Transaction.Current == null) && (SessionScope.Current == null)) || bNewCn)
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw;
            }
            finally
            {
                this.ReleaseParam(command);
            }
            return reader2;
        }

        private IDataReader ExecuteReader(DbCommand command, IEnumerable<SqlParam> paramList, CommandBehavior cmdBehavior, bool bNewCn = false)
        {
            ConnectionWrapper openConnection = null;
            IDataReader reader2;
            if (bNewCn)
            {
                openConnection = new ConnectionWrapper(this.GetNewOpenConnection(), true);
            }
            else
            {
                openConnection = this.GetOpenConnection(false);
            }
            try
            {
                PrepareCommand(command, openConnection.Connection);
                this.PrepareParameter(openConnection.Connection, command, paramList);
                if (bNewCn)
                {
                    return this.DoExecuteReader(command, CommandBehavior.CloseConnection | cmdBehavior);
                }
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    return this.DoExecuteReader(command, CommandBehavior.CloseConnection | cmdBehavior);
                }
                reader2 = this.DoExecuteReader(command, cmdBehavior);
            }
            catch (SqlException exception)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception.Message, exception);
            }
            catch (OracleException exception2)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception2.Message, exception2);
            }
            catch (NpgsqlException exception3)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception3.Message, exception3);
            }
            catch (MySqlException exception4)
            {
                if ((Transaction.Current == null) && (SessionScope.Current == null))
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw new ExceptionDatabase("BOS_ExecuteReader", command.CommandText, exception4.Message, exception4);
            }
            catch
            {
                if (((Transaction.Current == null) && (SessionScope.Current == null)) || bNewCn)
                {
                    openConnection.Connection.Close();
                    openConnection.Connection.Dispose();
                }
                throw;
            }
            finally
            {
                this.ReleaseParam(command);
            }
            return reader2;
        }

        public IDataReader ExecuteReaderWithNewCn(DbCommand cmd, IEnumerable<SqlParam> paramList)
        {
            return this.ExecuteReader(cmd, paramList, CommandBehavior.KeyInfo, true);
        }

        public IDataReader ExecuteReaderWithNewCn(DbCommand cmd, IEnumerable<SqlParam> paramList, CommandBehavior cmdBehavior)
        {
            return this.ExecuteReader(cmd, paramList, cmdBehavior, true);
        }

        public object ExecuteScalar(DbCommand command, params SqlParam[] paramList)
        {
            object obj2;
            try
            {
                using (ConnectionWrapper wrapper = this.GetOpenConnection())
                {
                    PrepareCommand(command, wrapper.Connection);
                    this.PrepareParameter(wrapper.Connection, command, paramList);
                    obj2 = this.DoExecuteScalar(command);
                }
            }
            catch (SqlException exception)
            {
                throw new ExceptionDatabase("BOS_ExecuteScalar", command.CommandText, exception.Message, exception);
            }
            catch (OracleException exception2)
            {
                throw new ExceptionDatabase("BOS_ExecuteScalar", command.CommandText, exception2.Message, exception2);
            }
            catch (NpgsqlException exception3)
            {
                throw new ExceptionDatabase("BOS_ExecuteScalar", command.CommandText, exception3.Message, exception3);
            }
            catch (MySqlException exception4)
            {
                throw new ExceptionDatabase("BOS_ExecuteScalar", command.CommandText, exception4.Message, exception4);
            }
            finally
            {
                this.ReleaseParam(command);
            }
            return obj2;
        }

        public List<SqlParam> ExecuteStoreProcedure(DbCommand command, IEnumerable<SqlParam> listParam = null)
        {
            List<SqlParam> list2;
            try
            {
                using (ConnectionWrapper wrapper = this.GetOpenConnection())
                {
                    PrepareCommand(command, wrapper.Connection);
                    this.PrepareParameter(wrapper.Connection, command, listParam);
                    command.ExecuteNonQuery();
                    List<SqlParam> list = new List<SqlParam>();
                    foreach (DbParameter parameter in command.Parameters)
                    {
                        if (parameter.Direction != ParameterDirection.Input)
                        {
                            SqlParam item = new SqlParam(parameter.ParameterName, (KDbType)parameter.DbType, parameter.Value);
                            list.Add(item);
                        }
                    }
                    list2 = list;
                }
            }
            catch (SqlException exception)
            {
                throw new ExceptionDatabase("BOS_ExecuteStoreProcedure", command.CommandText, exception.Message);
            }
            catch (OracleException exception2)
            {
                throw new ExceptionDatabase("BOS_ExecuteStoreProcedure", command.CommandText, exception2.Message);
            }
            catch (NpgsqlException exception3)
            {
                throw new ExceptionDatabase("BOS_ExecuteStoreProcedure", command.CommandText, exception3.Message);
            }
            catch (MySqlException exception4)
            {
                throw new ExceptionDatabase("BOS_ExecuteStoreProcedure", command.CommandText, exception4.Message);
            }
            finally
            {
                this.ReleaseParam(command);
            }
            return list2;
        }

        protected static int GetCommandTimeOut(int commandTimeout)
        {
            if (commandTimeout != 0)
            {
                if (_defaultCommandTimeOut == -1)
                {
                    string str = ConfigurationManager.AppSettings["DbCommandTimeout"];
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        _defaultCommandTimeOut = Convert.ToInt32(str);
                    }
                    else
                    {
                        _defaultCommandTimeOut = 30;
                    }
                }
                if (_defaultCommandTimeOut == 0)
                {
                    return 0;
                }
                if (commandTimeout < _defaultCommandTimeOut)
                {
                    return _defaultCommandTimeOut;
                }
            }
            return commandTimeout;
        }

        protected DbDataAdapter GetDataAdapter()
        {
            return this.CreateDataAdapter();
        }

        /// <summary>
        /// 获取新的连接
        /// </summary>
        /// <returns></returns>
        public DbConnection GetNewOpenConnection()
        {
            DbConnection connection = null;
            try
            {
                connection = this.CreateConnection();
                connection.Open();
            }
            catch
            {
                if (connection != null)
                {
                    connection.Close();
                }
                throw;
            }
            return connection;
        }

        protected ConnectionWrapper GetOpenConnection()
        {
            return this.GetOpenConnection(true);
        }

        protected ConnectionWrapper GetOpenConnection(bool disposeInnerConnection)
        {
            DbConnection connection = TransactionScopeConnections.GetConnection(this);
            if (connection != null)
            {
                return new ConnectionWrapper(connection, false);
            }
            return new ConnectionWrapper(this.GetNewOpenConnection(), disposeInnerConnection);
        }

        protected abstract DbParameter GetParameter(DbConnection conn, string name, KDbType custemDbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value);
        [Obsolete("该方法即将废弃")]
        protected abstract DbParameter GetParameter(DbConnection conn, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value, string udttypename = "");
        public DbConnection GetWrapConnection()
        {
            return this.GetOpenConnection().Connection;
        }

        protected virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = this.CreateCommandByCommandType(commandType, commandText, 0))
            {
                this.LoadDataSet(command, dataSet, tableNames, (IEnumerable<SqlParam>)null);
            }
        }

        protected virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, IEnumerable<SqlParam> listParam = null)
        {
            this.LoadDataSet(command, dataSet, new string[] { tableName }, listParam);
        }

        protected virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, IEnumerable<SqlParam> listParam = null)
        {
            using (ConnectionWrapper wrapper = this.GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                this.PrepareParameter(wrapper.Connection, command, listParam);
                this.DoLoadDataSet(command, dataSet, tableNames);
            }
        }

        protected virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, DbTransaction transaction)
        {
            this.LoadDataSet(command, dataSet, new string[] { tableName }, transaction);
        }

        protected virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            this.DoLoadDataSet(command, dataSet, tableNames);
        }

        protected void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = this.CreateCommandByCommandType(commandType, commandText, 0))
            {
                this.LoadDataSet(command, dataSet, tableNames, transaction);
            }
        }

        protected abstract int NonQueryByArray(DbCommand command, IEnumerable<SqlParam> listParam);
        protected static void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            command.Connection = connection;
        }

        protected static void PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;
        }

        protected void PrepareParameter(DbConnection conn, DbCommand cmd, IEnumerable<SqlParam> parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParam param in parameters)
                {
                    if ((int)param.KDbType==-9)
                    {
                        param.KDbType = KDbType.String;
                    }else if ((int)param.KDbType == -5)
                    {
                        param.KDbType = KDbType.Int64;
                    }
                    DbParameter parameter = this.GetParameter(conn, param.Name, param.KDbType, param.Size, param.Direction, true, 0, 0, string.Empty, DataRowVersion.Default, param.Value ?? DBNull.Value);

                    cmd.Parameters.Add(parameter);
                }
                
                this.ConvertTableFun(cmd);
            }
        }

        protected void PrepareParameter2(DbConnection conn, DbCommand cmd, object[] parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParam param in parameters)
                {
                    object value= param.Value;
                    if (value is string || value is char || value is char[])
                    {
                        param.KDbType = KDbType.String;
                    }
                    else if (value is int || value is long || value is short || value is byte)
                    {
                        param.KDbType = KDbType.Int32; // 或者根据具体类型选择DbType.Int64等
                    }
                    else if (value is float || value is double || value is decimal)
                    {
                        param.KDbType = KDbType.Decimal; // 或者根据具体类型选择DbType.Double等
                    }
                    else if (value is bool)
                    {
                        param.KDbType = KDbType.Boolean;
                    }
                    else if (value is DateTime)
                    {
                        param.KDbType = KDbType.DateTime;
                    }
                    else if (value is Guid)
                    {
                        param.KDbType = KDbType.Guid;
                    }else if(value is SqlParam)
                    {
                        param.Value = ((SqlParam)value).Value;
                        param.Name = ((SqlParam)value).Name;
                    }

                    DbParameter parameter = this.GetParameter(conn, param.Name, param.KDbType, param.Size, param.Direction, true, 0, 0, string.Empty, DataRowVersion.Default, param.Value ?? DBNull.Value);

                    cmd.Parameters.Add(parameter);
                }

                this.ConvertTableFun(cmd);
            }
        }


        protected virtual void ReleaseParam(IDbCommand cmd)
        {
        }

        public abstract bool TestDbOpenConnection(string strDbConnectionString, out string strErrMsg);

 

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }



        public Context CurContext { get; set; }
        public int DbType { get; set; }
    }
}
