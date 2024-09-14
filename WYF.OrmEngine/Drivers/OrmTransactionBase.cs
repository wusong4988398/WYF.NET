using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata.database;
using WYF.OrmEngine.DataManager;

namespace WYF.OrmEngine.Drivers
{
    public abstract class OrmTransactionBase : IOrmTransaction, IDbTransaction, IDisposable
    {

        private BulkCopyTaskContainer _bulkCopyTaskContainer;
       // private BulkUpdateTaskContainer _bulkUpdateTaskContainer;
        private IDbConnection _con;
        private static ObjectCache<DbMetadataTable, TableColumnConverterContainer> _converterCache = ObjectCache<DbMetadataTable, TableColumnConverterContainer>.Create();
        protected bool _hasCommitSqlTask;
        private List<IDatabaseTask> _lstSqlObj;
        private int _maxTableLevel;
        private int _minTableLevel;
        private IDbTransaction _tran;
        private const int ONCE_SQL_LENGTH = 0xfa0;

        public event EventHandler CommitAfter;
        protected OrmTransactionBase()
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

        private void AddRangeSqlTask(IEnumerable<IDatabaseTask> tasks)
        {
            this._lstSqlObj.AddRange(tasks);
            foreach (IDatabaseTask task in tasks)
            {
                int level = task.Level;
                if (this._minTableLevel > level)
                {
                    this._minTableLevel = level;
                }
                if (this._maxTableLevel < level)
                {
                    this._maxTableLevel = level;
                }
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

        internal static string AutoTableName(string tableName, string tableGroup)
        {
            if (string.IsNullOrEmpty(tableGroup))
            {
                return tableName;
            }
            return (tableName + "_" + tableGroup);
        }

        public virtual void Commit()
        {
            if (!this._hasCommitSqlTask)
            {
                this.CommitSqlTask();
            }
            if (this._tran != null)
            {
                this._tran.Commit();
            }
            this.FireCommitAfter();
        }

        public void CommitSqlTask()
        {
            this._hasCommitSqlTask = true;
            if (this._bulkCopyTaskContainer != null)
            {
                this.AddRangeSqlTask(this._bulkCopyTaskContainer.CreateTasks(this));
                this._bulkCopyTaskContainer = null;
            }
            //if (this._bulkUpdateTaskContainer != null)
            //{
            //    this.AddRangeSqlTask(this._bulkUpdateTaskContainer.CreateTasks(this));
            //    this._bulkUpdateTaskContainer = null;
            //}
            this.ExecuteSqlTasks(this._lstSqlObj);
        }

        protected virtual IDbConnection CreateAndOpenConnection()
        {
            throw new NotSupportedException("当启用了并行处理时，必须重载CreateConnection方法");
        }

        public virtual IDatabaseTask CreateBulkCopyTask(string tableName, ReadOnlyCollection<DbMetadataColumn> columns, IDataReader dataReader, int level)
        {
            throw new NotImplementedException("驱动程序需要重载此方法以实现批量处理。");
        }

        public virtual IDatabaseTask CreateBulkUpdateTask(string tableName, ReadOnlyCollection<DbMetadataColumn> readOnlyCollection, IDataReader dataReader, DbMetadataColumn pkColumn, object[] oids, int level)
        {
            throw new NotImplementedException("驱动程序需要重载此方法以实现批量处理。");
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

        internal static object DateTimeToDateTime(object obj)
        {
            DateTime time = (DateTime)obj;
            if (!(time == DateTime.MinValue))
            {
                return obj;
            }
            return DBNull.Value;
        }

        internal static object Decode(object obj)
        {
            string str = obj.ToString();
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return "0";
            }
            string s = "KingdeeK";
            string str3 = "KingdeeK";
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                byte[] rgbIV = Encoding.ASCII.GetBytes(str3);
                byte[] buffer = Convert.FromBase64String(str);
                string str4 = "";
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(stream2))
                            {
                                str4 = reader.ReadToEnd();
                            }
                        }
                    }
                }
                return str4;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public void Delete(DbMetadataTable table, object[] oids, object[] originalVersions, OperateOption option = null)
        {
            string str;
            object obj2;
            int tableLevel = GetTableLevel(table);
            List<object> list = new List<object>();
            SqlTask task = this.CreateSqlTask();
            string format = string.Format(this.GetPkFileter(oids, (DbType)table.PrimaryKey.DbType) + " where FID={0})", "{0}." + table.PrimaryKey.Name);
            switch ((DbType)table.PrimaryKey.DbType)
            {
                case DbType.Int16:
                    {
                        short[] numArray = new short[oids.Length];
                        for (int i = 0; i < oids.Length; i++)
                        {
                            numArray[i] = Convert.ToInt16(oids[i]);
                        }
                        obj2 = numArray;
                        break;
                    }
                case DbType.Int32:
                    {
                        int[] numArray2 = new int[oids.Length];
                        for (int j = 0; j < oids.Length; j++)
                        {
                            numArray2[j] = Convert.ToInt32(oids[j]);
                        }
                        obj2 = numArray2;
                        break;
                    }
                case DbType.Int64:
                    {
                        long[] numArray3 = new long[oids.Length];
                        for (int k = 0; k < oids.Length; k++)
                        {
                            numArray3[k] = Convert.ToInt64(oids[k]);
                        }
                        obj2 = numArray3;
                        break;
                    }
                case DbType.String:
                case DbType.AnsiString:
                    {
                        string[] strArray = new string[oids.Length];
                        for (int m = 0; m < oids.Length; m++)
                        {
                            strArray[m] = (string)oids[m];
                        }
                        obj2 = strArray;
                        break;
                    }
                default:
                    obj2 = oids;
                    break;
            }
            list.Add(task.AddUdtParamter("FID", (DbType)table.PrimaryKey.DbType, obj2, out str));
            object[] paramters = list.ToArray();
            foreach (string str3 in GetTableGroups(table))
            {
                string str4 = AutoTableName(table.Name, str3);
                SqlTask sqlTask = this.CreateSqlTask();
                sqlTask.Level = -((tableLevel * 2) - (string.IsNullOrEmpty(str3) ? 1 : 0));
                sqlTask.SqlBuilder.Append(" DELETE FROM " + str4 + " WHERE ");
                sqlTask.SqlBuilder.Append(string.Format(format, str4));
                sqlTask.AddParamters(paramters);
                this.AddSqlTask(sqlTask);
            }
        }

        public void Delete(IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, object[] rootOids, OperateOption option = null)
        {
            string str;
            object obj2;
            StringBuilder builder = new StringBuilder(0xfa0);
            List<object> list = new List<object>();
            SqlTask task = this.CreateSqlTask();
            builder.Append(this.GetPkTable(rootOids, (DbType)rootTable.PrimaryKey.DbType));
            switch ((DbType)rootTable.PrimaryKey.DbType)
            {
                case DbType.Int16:
                    {
                        short[] numArray = new short[rootOids.Length];
                        for (int i = 0; i < rootOids.Length; i++)
                        {
                            numArray[i] = Convert.ToInt16(rootOids[i]);
                        }
                        obj2 = numArray;
                        break;
                    }
                case DbType.Int32:
                    {
                        int[] numArray2 = new int[rootOids.Length];
                        for (int j = 0; j < rootOids.Length; j++)
                        {
                            numArray2[j] = Convert.ToInt32(rootOids[j]);
                        }
                        obj2 = numArray2;
                        break;
                    }
                case DbType.Int64:
                    {
                        long[] numArray3 = new long[rootOids.Length];
                        for (int k = 0; k < rootOids.Length; k++)
                        {
                            numArray3[k] = Convert.ToInt64(rootOids[k]);
                        }
                        obj2 = numArray3;
                        break;
                    }
                case DbType.String:
                case DbType.AnsiString:
                    {
                        string[] strArray = new string[rootOids.Length];
                        for (int m = 0; m < rootOids.Length; m++)
                        {
                            strArray[m] = (string)rootOids[m];
                        }
                        obj2 = strArray;
                        break;
                    }
                default:
                    obj2 = rootOids;
                    break;
            }
            list.Add(task.AddUdtParamter("FID", (DbType)rootTable.PrimaryKey.DbType, obj2, out str));
            object[] paramters = list.ToArray();
            foreach (DbMetadataTable table in tablesSchema)
            {
                
                if (table.SchemaType == DataSourceSchemaType.Table)
                {
                    List<string> tableGroups = GetTableGroups(table);
                    int tableLevel = GetTableLevel(table);
                    foreach (string str2 in tableGroups)
                    {
                        SqlTask sqlTask = this.CreateSqlTask();
                        sqlTask.SqlBuilder.AppendFormat(this.GetDeleteSql(rootTable, table, str2, builder.ToString()), builder.ToString());
                        sqlTask.Level = -((tableLevel * 2) - (string.IsNullOrEmpty(str2) ? 1 : 0));
                        sqlTask.AddParamters(paramters);
                        this.AddSqlTask(sqlTask);
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            if (this._tran != null)
            {
                this._tran.Dispose();
            }
            if (this._con != null)
            {
                this._con.Dispose();
            }
            foreach (IDatabaseTask task in this._lstSqlObj)
            {
                task.Dispose();
            }
        }

        internal static object Encode(object obj)
        {
            string str = ((decimal)obj).ToString();
            string s = "KingdeeK";
            string str3 = "KingdeeK";
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

        protected virtual void ExecuteSqlTasks(IList<IDatabaseTask> tasks)
        {
            for (int i = this._minTableLevel; i <= this._maxTableLevel; i++)
            {
                List<IDatabaseTask> lstSqlObj = new List<IDatabaseTask>();
                foreach (IDatabaseTask task in this._lstSqlObj)
                {
                    if (task.Level == i)
                    {
                        lstSqlObj.Add(task);
                    }
                }
                if (lstSqlObj.Count > 0)
                {
                    if (this.UseParallel && (lstSqlObj.Count > 3))
                    {
                        this.ParallelExecute(lstSqlObj);
                    }
                    else
                    {
                        IDbConnection con = this.Connection;
                        if (con == null)
                        {
                            con = this.CreateAndOpenConnection();
                        }
                        foreach (IDatabaseTask task2 in lstSqlObj)
                        {
                            task2.Execute(con, this._tran);
                        }
                    }
                }
            }
        }

        private void FireCommitAfter()
        {
            if (this.CommitAfter != null)
            {
                this.CommitAfter(this, EventArgs.Empty);
                this.CommitAfter = null;
            }
        }

        protected virtual Func<object, object> GetConverter(DbMetadataColumn col)
        {
            if (col.Encrypt)
            {
                return new Func<object, object>(OrmTransactionBase.Encode);
            }
            switch ((DbType)col.DbType)
            {
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTimeOffset:
                    if (col.ClrType == typeof(DateTime))
                    {
                        return new Func<object, object>(OrmTransactionBase.DateTimeToDateTime);
                    }
                    break;
            }
            return null;
        }

        private string GetDeleteSql(DbMetadataTable rootTable, DbMetadataTable currentTable, string tableGroup, string pkTable)
        {
            string str = AutoTableName(currentTable.Name, tableGroup);
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(" DELETE FROM {0} ", str);
            string str2 = this.GetDeleteWhere4DBType(rootTable, currentTable, pkTable, str);
            builder.Append(str2);
            return builder.ToString();
        }

        public virtual string GetDeleteWhere4DBType(DbMetadataTable rootTable, DbMetadataTable currentTable, string pkTable, string currentTableName)
        {
            DbMetadataRelation parentRelation = currentTable.ParentRelation;
            string str = "";
            if (parentRelation != null)
            {
                DbMetadataTable parentTable = currentTable;
                string name = currentTable.Name;
                string str3 = "";
                if ((parentRelation != null) && (parentRelation.ParentTable != rootTable))
                {
                    parentTable = parentRelation.ParentTable;
                    name = parentTable.Name;
                    parentRelation = parentTable.ParentRelation;
                    str3 = string.Format("  WHERE EXISTS(SELECT 1  FROM {0} ", name);
                    while ((parentRelation != null) && (parentRelation.ParentTable != rootTable))
                    {
                        DbMetadataTable table = parentRelation.ParentTable;
                        str = str + string.Format(" INNER JOIN {0}  ON {1}={2} ", table.Name, name + "." + parentRelation.ChildColumn.Name, table.Name + "." + table.PrimaryKey.Name);
                        parentTable = parentRelation.ParentTable;
                        name = parentTable.Name;
                        parentRelation = parentTable.ParentRelation;
                    }
                }
                if (str3.Length > 0)
                {
                    return (str3 + str + string.Format(" INNER JOIN {0} st ON st.FId={1}.{2} Where {3}.{4}={5}.{4})", new object[] { pkTable, name, parentRelation.ChildColumn.Name, currentTableName, currentTable.ParentRelation.ParentTable.PrimaryKey.Name, currentTable.ParentRelation.ParentTable.Name }));
                }
                return string.Format(" WHERE EXISTS(SELECT * FROM {0} where FId={1}.{2})", pkTable, currentTableName, parentRelation.ParentTable.PrimaryKey.Name);
            }
            return (str + string.Format(" WHERE EXISTS(SELECT * FROM {0} where FId={1}.{2})", pkTable, currentTableName, currentTable.PrimaryKey.Name));
        }

        private List<string> GetOids(object[] oids)
        {
            List<string> list = new List<string>();
            foreach (object obj2 in oids)
            {
                string item = obj2.ToString();
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        private string GetPkFileter(object[] oids, DbType dbtype)
        {
            string stype = "1";
            if (dbtype == DbType.AnsiString)
            {
                stype = "2";
            }
            else if (dbtype == DbType.String)
            {
                stype = "3";
            }
            return string.Format(" EXISTS (select 1 from  {0} ", this.GetPkTable(stype));
        }

        protected virtual string GetPkTable(string stype)
        {
            return (" table(fn_StrSplit(@FID, ','," + stype + ")) ");
        }

        private string GetPkTable(object[] oids, DbType dbtype)
        {
            string stype = "1";
            if (dbtype == DbType.AnsiString)
            {
                stype = "2";
            }
            else if (dbtype == DbType.String)
            {
                stype = "3";
            }
            return this.GetPkTable(stype);
        }

        private static SqlTask GetSqlBuilder(IDictionary<string, SqlTask> dictGroupSql, string tableGroup, SqlTask headSqlBuilderTupe)
        {
            if (string.IsNullOrEmpty(tableGroup))
            {
                return headSqlBuilderTupe;
            }
            return dictGroupSql[tableGroup];
        }

        internal TableColumnConverterContainer GetTableConverter(DbMetadataTable table, OperateOption option)
        {
            if (option.GetCacheMetadata())
            {
                return _converterCache.GetOrAdd(table, new Func<DbMetadataTable, TableColumnConverterContainer>(this.CreateTableConverter));
            }
            return this.CreateTableConverter(table);
        }

        private static List<string> GetTableGroups(DbMetadataTable currentTable)
        {
            List<string> list = new List<string>();
            foreach (DbMetadataColumn column in currentTable.Columns)
            {
                string item = column.TableGroup ?? "";
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            return list;
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

        private void Initialize()
        {
            this._lstSqlObj = new List<IDatabaseTask>();
        }

        public void Insert(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, OperateOption option = null)
        {
            if (this.SupportsBulkCopy && option.GetBulkCopy())
            {
                this.InsertByBulkCopy(table, inputValues, outputValues, oid, option);
            }
            else
            {
                this.InsertBySQL(table, inputValues, outputValues, oid, option);
            }
        }

        private void InsertByBulkCopy(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, OperateOption option)
        {
            if ((outputValues != null) && (outputValues.Length > 0))
            {
                ThrowNotSupportedOutput();
            }
            if (this._bulkCopyTaskContainer == null)
            {
                this._bulkCopyTaskContainer = new BulkCopyTaskContainer();
            }
            this._bulkCopyTaskContainer.Insert(table, inputValues, oid, option);
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

        private void ParallelExecute(List<IDatabaseTask> lstSqlObj)
        {
            int p = lstSqlObj.Count / 3;
            int p2 = p * 2;
            Parallel.For(0, 3, delegate (int i) {
                IDbConnection con = null;
                try
                {
                    con = this.CreateAndOpenConnection();
                    if (i == 0)
                    {
                        for (int n = 0; n < p; n++)
                        {
                            lstSqlObj[n].Execute(con, this._tran);
                        }
                    }
                    else if (i == 1)
                    {
                        for (int j = p; j < p2; j++)
                        {
                            lstSqlObj[j].Execute(con, this._tran);
                        }
                    }
                    else
                    {
                        for (int k = p2; k < lstSqlObj.Count; k++)
                        {
                            lstSqlObj[k].Execute(con, this._tran);
                        }
                    }
                }
                finally
                {
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            });
        }

        public virtual void Rollback()
        {
            if (this._tran != null)
            {
                this._tran.Rollback();
            }
        }

        private static void ThrowNotSupportedOutput()
        {
            throw new NotSupportedException("使用BulkCopy方式插入数据时，不支持字段上标记同步处理。");
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

        public void Update(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, IColumnValuePair originalVersion, OperateOption option = null)
        {
            if (this.SupportsBulkUpdate && option.GetBulkCopy())
            {
                this.UpdateByBulkCopy(table, inputValues, oid, originalVersion, option);
            }
            else
            {
                this.UpdateBySQL(table, inputValues, oid, originalVersion, option);
            }
        }

        private void UpdateByBulkCopy(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair oid, IColumnValuePair originalVersion, OperateOption option)
        {
            //if (this._bulkUpdateTaskContainer == null)
            //{
            //    this._bulkUpdateTaskContainer = new BulkUpdateTaskContainer();
            //}
            //this._bulkUpdateTaskContainer.Insert(table, inputValues, oid, option);
        }

        private void UpdateBySQL(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair oid, IColumnValuePair originalVersion, OperateOption option)
        {
            int tableLevel = GetTableLevel(table);
            SqlTask headSqlBuilderTupe = null;
            IDictionary<string, SqlTask> dictGroupSql = new Dictionary<string, SqlTask>();
            TableColumnConverterContainer tableConverter = this.GetTableConverter(table, option);
            DbMetadataColumn versionColumn = table.VersionColumn;
            int columnDbValue = 0;
            if (versionColumn != null)
            {
                columnDbValue = (int)tableConverter.GetColumnDbValue(originalVersion);
                originalVersion.Value = columnDbValue + 1;
                inputValues = TryAddColumnValuePair(inputValues, originalVersion);
            }
            foreach (IColumnValuePair pair in inputValues)
            {
                string item = pair.Column.TableGroup ?? string.Empty;
                if (((item != string.Empty) || (headSqlBuilderTupe == null)) && !dictGroupSql.Keys.Contains(item))
                {
                    SqlTask task2 = this.CreateSqlTask();
                    StringBuilder sqlBuilder = task2.SqlBuilder;
                    sqlBuilder.Append(" UPDATE ");
                    sqlBuilder.Append(AutoTableName(table.Name, item));
                    sqlBuilder.Append(" SET ");
                    if (item.Length == 0)
                    {
                        headSqlBuilderTupe = task2;
                    }
                    dictGroupSql.Add(item, task2);
                }
            }
            foreach (IColumnValuePair pair2 in inputValues)
            {
                string str3;
                DbMetadataColumn column = pair2.Column;
                string tableGroup = column.TableGroup ?? string.Empty;
                object obj2 = tableConverter.GetColumnDbValue(pair2);
                SqlTask task3 = GetSqlBuilder(dictGroupSql, tableGroup, headSqlBuilderTupe);
                task3.AddParamter(column.Name, (DbType)column.DbType, obj2, out str3);
                StringBuilder builder2 = task3.SqlBuilder;
                builder2.Append(column.Name);
                builder2.Append("=");
                builder2.Append(str3);
                builder2.Append(",");
            }
            foreach (KeyValuePair<string, SqlTask> pair3 in dictGroupSql)
            {
                string str4;
                StringBuilder builder3 = pair3.Value.SqlBuilder;
                builder3.Remove(builder3.Length - 1, 1);
                pair3.Value.AddParamter("OID", (DbType)table.PrimaryKey.DbType, tableConverter.GetColumnDbValue(oid), out str4);
                builder3.Append(" WHERE " + table.PrimaryKey.Name + "=" + str4);
                if ((versionColumn != null) && string.IsNullOrEmpty(pair3.Key))
                {
                    string str5;
                    pair3.Value.AddParamter("originalVersion", (DbType)versionColumn.DbType, columnDbValue, out str5);
                    builder3.Append(" AND " + versionColumn.Name + "=" + str5);
                }
                pair3.Value.Level = (tableLevel * 2) - (string.IsNullOrEmpty(pair3.Key) ? 1 : 0);
                pair3.Value.ExpectedAffectedCount = 1;
                this.AddSqlTask(pair3.Value);
            }
        }

        // Properties
        public virtual IDbConnection Connection
        {
            get
            {
                return this._con;
            }
        }

        protected abstract DbProviderFactory DbProviderFactory { get; }

        public  virtual bool ExTableHaveRelitionField
        {
            get
            {
                return false;
            }
        }

        public virtual IsolationLevel IsolationLevel
        {
            get
            {
                if (this._tran == null)
                {
                    throw new NotImplementedException();
                }
                return this._tran.IsolationLevel;
            }
        }

        public virtual bool SupportsBulkCopy
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsBulkUpdate
        {
            get
            {
                return false;
            }
        }

        protected virtual bool UseParallel
        {
            get
            {
                return false;
            }
        }


        private sealed class PrivateSqlTask : SqlTask
        {
    
            public PrivateSqlTask(IDbCommand cmd) : base(cmd)
            {
            }
        }
    }
}
