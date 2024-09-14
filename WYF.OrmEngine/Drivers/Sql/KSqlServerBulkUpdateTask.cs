using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;

namespace WYF.OrmEngine.Drivers.Sql
{
    internal class KSqlServerBulkUpdateTask : BulkCopyTaskBase
    {

        private Context _ctx;
        private object[] _oids;
        private DbMetadataColumn _pkColumn;


        public KSqlServerBulkUpdateTask(Context ctx, string tableName, ReadOnlyCollection<DbMetadataColumn> columns, IDataReader dataReader, DbMetadataColumn pkColumn, object[] oids, int level) : base(tableName, columns, dataReader, level)
        {
            this._ctx = ctx;
            this._oids = oids;
            this._pkColumn = pkColumn;
        }

        private DataTable CreateBatchTable()
        {
            bool flag = false;
            DataTable table = new DataTable();
            table.BeginInit();
            for (int i = 0; i < base.Columns.Count; i++)
            {
                DbMetadataColumn column = base.Columns[i];
                if (column.DbType == (int)DbType.DateTime)
                {
                    table.Columns.Add(column.Name, typeof(DateTime));
                }
                else if (((column.DbType != (int)DbType.StringFixedLength) && (column.DbType != (int)DbType.AnsiStringFixedLength)) && ((DbType)column.DbType != DbType.String))
                {
                    table.Columns.Add(column.Name, column.ClrType);
                }
                else
                {
                    table.Columns.Add(column.Name, typeof(string));
                }
                if (column.Name.Equals(this._pkColumn.Name, StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                table.Columns.Add(this._pkColumn.Name, this._pkColumn.ClrType);
            }
            table.EndInit();
            using (IDataReader reader = base.DataReader)
            {
                for (int j = 0; reader.Read(); j++)
                {
                    int num3 = 0;
                    DataRow row = table.NewRow();
                    num3 = 0;
                    while (num3 < base.Columns.Count)
                    {
                        row[num3] = reader.GetValue(num3) ?? DBNull.Value;
                        num3++;
                    }
                    if (!flag)
                    {
                        row[num3] = this._oids[j];
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public override void Execute(IDbConnection con, IDbTransaction tran)
        {
            if (base.DataReader.RecordsAffected > 1)
            {
                this.ExecuteBatch(con, tran);
            }
            else
            {
                this.ExecuteByRow(con, tran);
            }
        }

        private void ExecuteBatch(IDbConnection con, IDbTransaction tran)
        {
            DataTable dt = this.CreateBatchTable();
            BatchSqlParam param = new BatchSqlParam(base.TableName, dt, "")
            {
                DataBaseType = this._ctx.DatabaseType
            };
            for (int i = 0; i < base.Columns.Count; i++)
            {
                DbMetadataColumn column = base.Columns[i];
                param.AddSetExpression(column.Name, TypeToKDbType((DbType)column.DbType), column.Name);
            }
            param.AddWhereExpression(this._pkColumn.Name, TypeToKDbType((DbType)this._pkColumn.DbType), this._pkColumn.Name, "");
            DatabaseFactory.CreateDataBase(this._ctx).BatchUpdate(param, "");
        }

        private void ExecuteByRow(IDbConnection con, IDbTransaction tran)
        {
            AbstractDatabase database = (AbstractDatabase)DatabaseFactory.CreateDataBase(this._ctx);
            string dBParameterPre = this.GetDBParameterPre();
            using (DbCommand command = database.CreateCommandByCommandType(CommandType.Text, string.Empty, 0))
            {
                DbParameter parameter;
                string str2 = string.Empty;
                for (int i = 0; i < base.Columns.Count; i++)
                {
                    DbMetadataColumn column = base.Columns[i];
                    parameter = command.CreateParameter();
                    parameter.ParameterName = dBParameterPre + column.Name;
                    parameter.DbType = (DbType)column.DbType;
                    command.Parameters.Add(parameter);
                    if (str2.Length > 0)
                    {
                        str2 = str2 + ",";
                    }
                    string str4 = str2;
                    str2 = str4 + column.Name + "=" + dBParameterPre + column.Name;
                }
                parameter = command.CreateParameter();
                parameter.DbType = (DbType)this._pkColumn.DbType;
                parameter.Value = this._oids;
                parameter.ParameterName = dBParameterPre + "PkValue";
                command.Parameters.Add(parameter);
                string str3 = "Update " + base.TableName + " SET " + str2 + " WHERE " + this._pkColumn.Name + "=" + parameter.ParameterName;
                command.CommandText = str3;
                using (IDataReader reader = base.DataReader)
                {
                    for (int j = 0; reader.Read(); j++)
                    {
                        int num3 = 0;
                        num3 = 0;
                        while (num3 < base.Columns.Count)
                        {
                            command.Parameters[num3].Value = reader.GetValue(num3) ?? DBNull.Value;
                            num3++;
                        }
                        command.Parameters[num3].Value = this._oids[j];
                        database.ExecuteNonQuery(command, null);
                    }
                }
            }
        }

        protected virtual string GetDBParameterPre()
        {
            return "@";
        }

        internal static KDbType TypeToKDbType(DbType t)
        {
            try
            {
                return (KDbType)Enum.ToObject(typeof(KDbType), (int)t);
            }
            catch
            {
                return KDbType.Object;
            }
        }
    }
}
