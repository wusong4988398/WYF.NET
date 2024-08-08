using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.database;
using WYF.Bos.Orm.datamanager;
using JNPF.Form.DataEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Drivers
{
    public abstract class OrmTransactionBase : IOrmTransaction, IDbTransaction, IDisposable
    {
        private IDbConnection _con;
        private IDbTransaction _tran;
        private List<IDatabaseTask> _lstSqlObj;
        private int _maxTableLevel;
        private int _minTableLevel;
        private static ObjectCache<DbMetadataTable, TableColumnConverterContainer> _converterCache = ObjectCache<DbMetadataTable, TableColumnConverterContainer>.Create();
        protected abstract DbProviderFactory DbProviderFactory { get; }

        public OrmTransactionBase()
        {
            this._minTableLevel = 1;
            this._maxTableLevel = 1;
            this.Initialize();
        }

        protected OrmTransactionBase(IDbConnection con, IDbTransaction tran)
        {
            this._minTableLevel = 1;
            this._maxTableLevel = 1;
            this._con = con;
            this._tran = tran;
            this.Initialize();
        }

        private void Initialize()
        {
            this._lstSqlObj = new List<IDatabaseTask>();
        }

        public virtual IDbConnection Connection
        {
            get
            {
                return this._con;
            }
        }

        public virtual bool SupportsBulkCopy
        {
            get
            {
                return false;
            }
        }

        public IsolationLevel IsolationLevel => throw new NotImplementedException();

        public event EventHandler CommitAfter;

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void CommitSqlTask()
        {
            throw new NotImplementedException();
        }

        public void Delete(DbMetadataTable table, object[] oids, object[] originalVersions, OperateOption option = null)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, object[] rootOids, OperateOption option = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Insert(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, OperateOption option = null)
        {
            
            if (this.SupportsBulkCopy && option.GetBulkCopy())
            {
               // this.InsertByBulkCopy(table, inputValues, outputValues, oid, option);
            }
            else
            {
                this.InsertBySQL(table, inputValues, outputValues, oid, option);
            }
        }


        private void InsertBySQL(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, OperateOption option)
        {
            int tableLevel = GetTableLevel(table);
            SqlTask headSqlBuilderTupe = null;
            IDictionary<string, SqlTask> dictGroupSql = new Dictionary<string, SqlTask>();
            TableColumnConverterContainer tableConverter = this.GetTableConverter(table, option);
            if (table.VersionColumn != null)
            {
                inputValues = TryAddColumnValuePair(inputValues, new SimpleColumnValuePair(table.VersionColumn, 0));
            }
            IColumnValuePair pair = null;
            if (this.ExTableHaveRelitionField && (table.ParentRelation != null))
            {
                DbMetadataColumn childColumn = table.ParentRelation.ChildColumn;
                foreach (IColumnValuePair pair2 in inputValues)
                {
                    if (pair2.Column.ColumnIndex == childColumn.ColumnIndex)
                    {
                        pair = pair2;
                        break;
                    }
                }
            }
            foreach (DbMetadataColumn column2 in table.Columns)
            {
                string item = column2.TableGroup ?? string.Empty;
                if (((item != string.Empty) || (headSqlBuilderTupe == null)) && !dictGroupSql.Keys.Contains(item))
                {
                    SqlTask task2 = this.CreateSqlTask();
                    StringBuilder sqlBuilder = task2.SqlBuilder;
                    sqlBuilder.Append(" INSERT INTO ");
                    sqlBuilder.Append(AutoTableName(table.Name, item));
                    sqlBuilder.Append("(");

                    if (item.Length == 0)
                    {
                        headSqlBuilderTupe = task2;
                    }
                    else
                    {
                        sqlBuilder.Append(table.PrimaryKey.Name);
                        sqlBuilder.Append(",");
                        if (pair != null)
                        {
                            sqlBuilder.Append(pair.Column.Name);
                            sqlBuilder.Append(",");
                        }
                    }
                    dictGroupSql.Add(item, task2);
                }

            }

            foreach (IColumnValuePair pair3 in inputValues)
            {
                DbMetadataColumn column = pair3.Column;
                string tableGroup = column.TableGroup ?? string.Empty;
                StringBuilder builder2 = GetSqlBuilder(dictGroupSql, tableGroup, headSqlBuilderTupe).SqlBuilder;
                builder2.Append(column.Name);
                builder2.Append(",");
            }
            foreach (KeyValuePair<string, SqlTask> pair4 in dictGroupSql)
            {
                StringBuilder builder3 = pair4.Value.SqlBuilder;
                builder3.Remove(builder3.Length - 1, 1);
                builder3.Append(") VALUES (");
                if (pair4.Key.Length > 0)
                {
                    string str3;
                    pair4.Value.AddParamter(table.PrimaryKey.Name, (DbType)table.PrimaryKey.DbType, oid.Value, out str3);
                    builder3.Append(str3);
                    builder3.Append(",");
                    if (pair != null)
                    {
                        DbMetadataColumn column4 = pair.Column;
                        pair4.Value.AddParamter(column4.Name, (DbType)column4.DbType, pair.Value, out str3);
                        builder3.Append(str3);
                        builder3.Append(",");
                    }
                }
            }
            foreach (IColumnValuePair pair5 in inputValues)
            {
                string str5;
                DbMetadataColumn column5 = pair5.Column;
                string str4 = column5.TableGroup ?? string.Empty;
                object columnDbValue = tableConverter.GetColumnDbValue(pair5);
                SqlTask task3 = GetSqlBuilder(dictGroupSql, str4, headSqlBuilderTupe);
                task3.AddParamter(column5.Name, (DbType)column5.DbType, columnDbValue, out str5);
                task3.SqlBuilder.Append(str5);
                task3.SqlBuilder.Append(",");
            }
            foreach (KeyValuePair<string, SqlTask> pair6 in dictGroupSql)
            {
                StringBuilder builder4 = pair6.Value.SqlBuilder;
                builder4.Remove(builder4.Length - 1, 1);
                builder4.Append(")");
                pair6.Value.Level = (tableLevel * 2) - (string.IsNullOrEmpty(pair6.Key) ? 1 : 0);
                pair6.Value.ExpectedAffectedCount = 1;
                this.AddSqlTask(pair6.Value);
            }

        }


        private void AddSqlTask(SqlTask sqlTask)
        {
            this._lstSqlObj.Add(sqlTask);
            int level = sqlTask.Level;
            if (this._minTableLevel > level)
            {
                this._minTableLevel = level;
            }
            if (this._maxTableLevel < level)
            {
                this._maxTableLevel = level;
            }
        }


        private static SqlTask GetSqlBuilder(IDictionary<string, SqlTask> dictGroupSql, string tableGroup, SqlTask headSqlBuilderTupe)
        {
            if (string.IsNullOrEmpty(tableGroup))
            {
                return headSqlBuilderTupe;
            }
            return dictGroupSql[tableGroup];
        }
        internal static string AutoTableName(string tableName, string tableGroup)
        {
            if (string.IsNullOrEmpty(tableGroup))
            {
                return tableName;
            }
            return (tableName + "_" + tableGroup);
        }
        protected virtual SqlTask CreateSqlTask()
        {
            DbProviderFactory dbProviderFactory = this.DbProviderFactory;
            if (dbProviderFactory == null)
            {
                throw new NotImplementedException();
            }
            return new PrivateSqlTask(dbProviderFactory.CreateCommand());
        }
        private static IColumnValuePair[] TryAddColumnValuePair(IColumnValuePair[] inputValues, IColumnValuePair addpair)
        {
            int columnIndex = addpair.Column.ColumnIndex;
            for (int i = 0; i < inputValues.Length; i++)
            {
                if (inputValues[i].Column.ColumnIndex == columnIndex)
                {
                    return inputValues;
                }
            }
            IColumnValuePair[] array = new IColumnValuePair[inputValues.Length + 1];
            inputValues.CopyTo(array, 0);
            array[array.Length - 1] = addpair;
            return array;
        }

        internal TableColumnConverterContainer GetTableConverter(DbMetadataTable table, OperateOption option)
        {
            if (option.GetCacheMetadata())
            {
                return _converterCache.GetOrAdd(table, new Func<DbMetadataTable, TableColumnConverterContainer>(this.CreateTableConverter));
            }
            return this.CreateTableConverter(table);
        }
        protected internal virtual bool ExTableHaveRelitionField
        {
            get
            {
                return false;
            }
        }
        private TableColumnConverterContainer CreateTableConverter(DbMetadataTable table)
        {
            DbMetadataColumnCollection columns = table.Columns;
            Func<object, object>[] converters = new Func<object, object>[columns.Count];
            for (int i = 0; i < columns.Count; i++)
            {
                converters[i] = this.GetConverter(columns[i]);
            }
            return new TableColumnConverterContainer(converters);
        }
        protected virtual Func<object, object> GetConverter(DbMetadataColumn col)
        {
            if (col.Encrypt)
            {
                return new Func<object, object>(OrmTransactionBase.Encode);
            }
            switch (col.DbType)
            {
                //case DbType.Date:
                //case DbType.DateTime:
                //case DbType.DateTimeOffset:
                //    if (col.ClrType == typeof(DateTime))
                //    {
                //        return new Func<object, object>(OrmTransactionBase.DateTimeToDateTime);
                //    }
                //    break;
            }
            return null;
        }

        internal static object DateTimeToDateTime(object obj)
        {
            DateTime time = (DateTime)obj;
            if (!(time == DateTime.MinValue))
            {
                return obj;
            }
            return DBNull.Value;
        }
        internal static int GetTableLevel(DbMetadataTable currentTable)
        {
            int num = 1;
            DbMetadataRelation parentRelation = currentTable.ParentRelation;
            DbMetadataTable table = currentTable;
            while (parentRelation != null)
            {
                parentRelation = parentRelation.ParentTable.ParentRelation;
                num++;
            }
            return num;
        }
        internal static object Encode(object obj)
        {
            string str = ((decimal)obj).ToString();
            string s = "seasoft";
            string str3 = "seasoft";
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                byte[] rgbIV = Encoding.ASCII.GetBytes(str3);
                byte[] inArray = null;
                int length = 0;
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    int keySize = provider.KeySize;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(bytes, rgbIV), CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(stream2))
                            {
                                writer.Write(str);
                                writer.Flush();
                                stream2.FlushFinalBlock();
                                writer.Flush();
                                inArray = stream.GetBuffer();
                                length = (int)stream.Length;
                            }
                        }
                    }
                }
                return Convert.ToBase64String(inArray, 0, length);
            }
            catch
            {
                return "Convert Error";
            }
        }
        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public void Update(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, IColumnValuePair originalVersion, OperateOption option = null)
        {
            throw new NotImplementedException();
        }

        private sealed class PrivateSqlTask : SqlTask
        {
          
            public PrivateSqlTask(IDbCommand cmd) : base(cmd)
            {
            }
        }
    }
}
