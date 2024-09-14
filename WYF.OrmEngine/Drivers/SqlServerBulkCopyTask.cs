using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.Drivers
{
    public class SqlServerBulkCopyTask : BulkCopyTaskBase
    {
        
        public SqlServerBulkCopyTask(string tableName, ReadOnlyCollection<DbMetadataColumn> columns, IDataReader dataReader, int level) : base(tableName, columns, dataReader, level)
        {
        }

        public override void Execute(IDbConnection con, IDbTransaction tran)
        {
            SqlConnection connection = (SqlConnection)con;
            SqlTransaction externalTransaction = (SqlTransaction)tran;
            using (SqlBulkCopy copy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, externalTransaction))
            {
                copy.DestinationTableName = base.TableName;
                string[] columns = GetColumnsFromDatabase(connection, externalTransaction, base.TableName);
                foreach (DbMetadataColumn column in base.Columns)
                {
                    copy.ColumnMappings.Add(column.Name, MappingToDatabaseColumn(column.Name, base.TableName, columns));
                }
                try
                {
                    copy.WriteToServer(base.DataReader);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    base.DataReader.Close();
                }
            }
        }

        private static string[] GetColumnsFromDatabase(SqlConnection con, SqlTransaction sqlTran, string tableName)
        {
            string cmdText = "SELECT TOP 1 * FROM [" + tableName + "] WHERE 1=0";
            List<string> list = new List<string>();
            using (SqlCommand command = new SqlCommand(cmdText, con))
            {
                if (sqlTran != null)
                {
                    command.Transaction = sqlTran;
                }
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        list.Add(reader.GetName(i));
                    }
                    reader.Close();
                }
            }
            return list.ToArray();
        }

        private static string MappingToDatabaseColumn(string source, string tableName, string[] columns)
        {
            foreach (string str in columns)
            {
                if (string.Equals(str, source, StringComparison.OrdinalIgnoreCase))
                {
                    return str;
                }
            }
            ThrowNotFind(source, tableName, columns);
            return null;
        }

        private static void ThrowNotFind(string source, string tableName, string[] columns)
        {
            string str = string.Empty;
            foreach (string str2 in columns)
            {
                if (str.Length > 0)
                {
                    str = str + ",";
                }
                str = str + str2;
            }
            throw new FieldAccessException(string.Format("在进行批量插入时，实体提供的列 {0} 在表 {1} 中无法找到。数据库现有列为：{2}", source, tableName, str));
        }
    }
}
