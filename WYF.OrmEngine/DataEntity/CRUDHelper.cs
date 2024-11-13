using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine.db;
using WYF.OrmEngine.dataManager;
using WYF.OrmEngine.Drivers;

namespace WYF.OrmEngine.DataEntity
{
    public class CRUDHelper
    {
        protected static SqlTask CreateSqlTask(DBRoute dbRoute, int level)
        {
            //return new SqlTask(dbRoute, level);
            throw new NotImplementedException();
        }

        public static Collection<SqlTask> Insert(DBRoute dbRoute, DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid)
        {
            int tableLevel = GetTableLevel(table);
            SqlTask headSqlBuilderTupe = null;
            Dictionary<string, SqlTask> dictGroupSql = new Dictionary<string, SqlTask>();
            TableColumnConverterContainer columnConverter = GetTableConverter(table);

            if (table.VersionColumn != null)
            {
                inputValues = TryAddColumnValuePair(inputValues, new SimpleColumnValuePair(table.VersionColumn, 0));
            }

            IColumnValuePair exTableParentColumnValuePair = null;
            if (GetExTableHaveRelitionField() && table.ParentRelation != null)
            {
                DbMetadataColumn exTableParentColumn = table.ParentRelation.ChildColumn;
                foreach (IColumnValuePair pair in inputValues)
                {
                    if (pair.Column.ColumnIndex == exTableParentColumn.ColumnIndex)
                    {
                        exTableParentColumnValuePair = pair;
                        break;
                    }
                }
            }

            foreach (DbMetadataColumn column in table.Columns)
            {
                string tableGroup = column.TableGroup ?? "";
                if (tableGroup.Length == 0 && headSqlBuilderTupe != null)
                    continue;

                if (!dictGroupSql.ContainsKey(tableGroup))
                {
                    string tableName = GetTableNameWithGroup(table.Name, tableGroup);
                    SqlTask sqlTask = CreateSqlTask(dbRoute, tableLevel);
                    StringBuilder sqlBuilder = sqlTask.SqlBuilder;
                    sqlBuilder.Append("INSERT INTO ");
                    sqlBuilder.Append(tableName);
                    sqlBuilder.Append('(');

                    if (tableGroup.Length == 0)
                    {
                        headSqlBuilderTupe = sqlTask;
                    }
                    else
                    {
                        sqlBuilder.Append(table.PrimaryKey.Name);
                        sqlBuilder.Append(',');

                        if (exTableParentColumnValuePair != null)
                        {
                            sqlBuilder.Append(exTableParentColumnValuePair.Column.Name);
                            sqlBuilder.Append(',');
                        }
                    }

                    dictGroupSql[tableGroup] = sqlTask;
                }
            }

            foreach (IColumnValuePair tuple in inputValues)
            {
                DbMetadataColumn column = tuple.Column;
                string tableGroup = column.TableGroup ?? "";
                StringBuilder sqlBuilder = GetSqlBuilder(dictGroupSql, tableGroup, headSqlBuilderTupe).SqlBuilder;
                sqlBuilder.Append(column.Name);
                sqlBuilder.Append(',');
            }

            foreach (KeyValuePair<string, SqlTask> item in dictGroupSql)
            {
                StringBuilder sql = item.Value.SqlBuilder;
                sql.Remove(sql.Length - 1, 1); // 删除最后一个逗号
                sql.Append(") VALUES (");

                if (item.Key.Length > 0)
                {
                    item.Value.AddParameter(table.PrimaryKey.Name, (DbType)table.PrimaryKey.DbType, oid.Value);
                    sql.Append('?');
                    sql.Append(',');

                    if (exTableParentColumnValuePair != null)
                    {
                        DbMetadataColumn column = exTableParentColumnValuePair.Column;
                        item.Value.AddParameter(column.Name, (DbType)column.DbType, exTableParentColumnValuePair.Value);
                        sql.Append('?');
                        sql.Append(',');
                    }
                }
            }

            foreach (IColumnValuePair tuple in inputValues)
            {
                DbMetadataColumn column = tuple.Column;
                string tableGroup = column.TableGroup ?? "";
                object value = columnConverter.GetColumnDbValue(tuple);
                SqlTask sqlBuilderTupe = GetSqlBuilder(dictGroupSql, tableGroup, headSqlBuilderTupe);
                sqlBuilderTupe.AddParameter(column.Name, (DbType)column.DbType, value);
                sqlBuilderTupe.SqlBuilder.Append('?');
                sqlBuilderTupe.SqlBuilder.Append(',');
            }

            foreach (KeyValuePair<string, SqlTask> item in dictGroupSql)
            {
                StringBuilder sql = item.Value.SqlBuilder;
                sql.Remove(sql.Length - 1, 1); // 删除最后一个逗号
                sql.Append(')');
            }

            return new Collection<SqlTask>(dictGroupSql.Values.ToArray());
        }



        public static ICollection<SqlTask> Update(DBRoute dbRoute, DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair oid, IColumnValuePair originalVersion)
        {
            SqlTask headSqlBuilderTupe = null;
            Dictionary<string, SqlTask> dictGroupSql = new Dictionary<string, SqlTask>();
            TableColumnConverterContainer columnConverter = GetTableConverter(table);
            DbMetadataColumn versionColumn = table.VersionColumn;
            int originalVersionValue = 0;
            if (versionColumn != null)
            {
                if (originalVersion == null)
                {
                    throw new ArgumentNullException(nameof(originalVersion), "Original version cannot be null when version column is present.");
                }
                originalVersionValue = (int)columnConverter.GetColumnDbValue(originalVersion);
                originalVersion.Value = originalVersionValue + 1;
                inputValues = TryAddColumnValuePair(inputValues, originalVersion);
            }

            int tableLevel = GetTableLevel(table);

            foreach (IColumnValuePair tuple in inputValues)
            {
                string tableGroup = tuple.Column.TableGroup ?? "";
                if (headSqlBuilderTupe == null || tableGroup.Length != 0)
                {
                    if (!dictGroupSql.ContainsKey(tableGroup))
                    {
                        string tableName = GetTableNameWithGroup(table.Name, tableGroup);
                        SqlTask sqlTask = CreateSqlTask(dbRoute, tableLevel);
                        StringBuilder sb = sqlTask.SqlBuilder;
                        sb.Append("UPDATE ");
                        sb.Append(tableName);
                        sb.Append(" SET ");
                        if (tableGroup.Length == 0)
                        {
                            headSqlBuilderTupe = sqlTask;
                        }
                        dictGroupSql[tableGroup] = sqlTask;
                    }
                }
            }

            foreach (IColumnValuePair tuple in inputValues)
            {
                DbMetadataColumn column = tuple.Column;
                string tableGroup = column.TableGroup ?? "";
                object value = columnConverter.GetColumnDbValue(tuple);
                SqlTask sqlTask = GetSqlBuilder(dictGroupSql, tableGroup, headSqlBuilderTupe);
                sqlTask.AddParameter(column.Name, (DbType)column.DbType, value);
                StringBuilder sb = sqlTask.SqlBuilder;
                sb.Append(column.Name);
                sb.Append('=');
                sb.Append('?');
                sb.Append(',');
            }

            foreach (KeyValuePair<string, SqlTask> item in dictGroupSql)
            {
                StringBuilder sql = item.Value.SqlBuilder;
                sql.Remove(sql.Length - 1, 1); // 删除最后一个逗号
                item.Value.AddParameter("OID", (DbType)table.PrimaryKey.DbType, columnConverter.GetColumnDbValue(oid));
                sql.Append(" WHERE ").Append(table.PrimaryKey.Name).Append("=?");
                if (versionColumn != null && string.IsNullOrEmpty(item.Key))
                {
                    item.Value.AddParameter("originalVersion", (DbType)versionColumn.DbType, originalVersionValue);
                    sql.Append(" AND ").Append(versionColumn.Name).Append("=?");
                }
            }

            return dictGroupSql.Values;
        }

        public static List<SqlTask> Delete(DBRoute dbRoute, DbMetadataTable table, object[] oids, object[] originalVersions)
        {
            int tableLevel = GetTableLevel(table);
            List<SqlParameter> pks = GetPKs(oids, table.PrimaryKey.DbType);
            List<string> tableGroupList = GetTableGroups(table);
            List<SqlTask> tasks = new List<SqlTask>();

            foreach (string tableGroup in tableGroupList)
            {
                string tableName = GetTableNameWithGroup(table.Name, tableGroup);
                SqlTask sqlTask = CreateSqlTask(dbRoute, tableLevel);
                sqlTask.SqlBuilder.Append("DELETE FROM ").Append(tableName).Append(" WHERE ")
                    .Append(table.PrimaryKey.Name).Append(InParameter(pks.Count));
               // sqlTask.GetParameters().AddRange(pks);
          
                tasks.Add(sqlTask);
            }

            return tasks;
        }

        public static int Delete(DBRoute dbRoute, IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, object[] rootOids)
        {
            List<SqlParameter> pks = GetPKs(rootOids, rootTable.PrimaryKey.DbType);
            Stack<SqlTask> sqlList = new Stack<SqlTask>();

            foreach (DbMetadataTable table in tablesSchema)
            {
                foreach (string tableGroup in GetTableGroups(table))
                {
                    SqlTask sqlTask = CreateSqlTask(dbRoute, 0);
                    GetDeleteSql(rootTable, table, tableGroup, pks, sqlTask);
                    sqlList.Push(sqlTask);
                }
            }

            int count = 0;
            while (sqlList.Count > 0)
            {
                count += ExecuteSqlTask(sqlList.Pop());
            }

            return count;
        }

        private static int ExecuteSqlTask(SqlTask sqlTask)
        {
            throw new NotImplementedException();
            //return sqlTask.Execute();
        }

        private static List<SqlParameter> GetPKs(object[] oids, int dbType)
        {
            List<SqlParameter> sps = new List<SqlParameter>(oids.Length);
            if (oids.Length > 0)
            {
                foreach (object oid in oids)
                {
                    object pkValue = oid is DynamicObject ? ((DynamicObject)oid).PkValue: oid;
                    sps.Add(new SqlParameter("@OID", (DbType)dbType, pkValue));

                }
            }
            else
            {
                object defaultValue = dbType switch
                {
                    12 or -9 or -15 => "", // VARCHAR, NVARCHAR, NCHAR
                    _ => 0 // 其他类型，默认为整数0
                };
                sps.Add(new SqlParameter("@OID", (DbType)dbType, defaultValue));

            }
            return sps;
        }


        private static void GetDeleteSql(DbMetadataTable rootTable, DbMetadataTable currentTable, string tableGroup, List<SqlParameter> pks, SqlTask sqlTask)
        {
            string currentTableName = GetTableNameWithGroup(currentTable.Name, tableGroup);
            sqlTask.SqlBuilder.Append("DELETE FROM ").Append(currentTableName);
            DbMetadataRelation parentRelation = currentTable.ParentRelation;

            if (parentRelation != null)
            {
                DbMetadataTable childTable = currentTable;
                string childTableName = currentTable.Name;

                if (parentRelation.ParentTable != rootTable)
                {
                    if (parentRelation.ParentTable.ParentRelation?.ParentTable == rootTable)
                    {
                        childTable = parentRelation.ParentTable;
                        childTableName = childTable.Name;
                        parentRelation = childTable.ParentRelation;
                        sqlTask.SqlBuilder.Append(string.Format(" WHERE {0} IN (SELECT {0} FROM {1} WHERE {2} {3})",
                            parentRelation.ParentTable.PrimaryKey.Name, childTableName, parentRelation.ChildColumn.Name, InParameter(pks.Count)));
                        //sqlTask.GetParameters().AddRange(pks);
                    }
                    else
                    {
                        childTable = parentRelation.ParentTable;
                        childTableName = childTable.Name;
                        parentRelation = childTable.ParentRelation;
                        sqlTask.SqlBuilder.Append(string.Format(" WHERE {0} IN (SELECT {0} FROM {1} WHERE {2} IN (SELECT {2} FROM {3} WHERE {4} {5}))",
                            parentRelation.ParentTable.PrimaryKey.Name, childTableName, parentRelation.ChildColumn.Name, parentRelation.ParentTable.Name,
                            parentRelation.ParentTable.ParentRelation.ChildColumn.Name, InParameter(pks.Count)));
                        //sqlTask.GetParameters().AddRange(pks);
                    }
                }
                else
                {
                    sqlTask.SqlBuilder.Append(" WHERE ").Append(parentRelation.ParentTable.PrimaryKey.Name).Append(InParameter(pks.Count));
                    //sqlTask.GetParameters().AddRange(pks);
                }
            }
            else
            {
                sqlTask.SqlBuilder.Append(" WHERE ").Append(currentTable.PrimaryKey.Name).Append(InParameter(pks.Count));
                //sqlTask.GetParameters().AddRange(pks);
            }
        }



        private static List<string> GetTableGroups(DbMetadataTable currentTable)
        {
            List<string> result = new List<string>();
            foreach (DbMetadataColumn column in currentTable.Columns)
            {
                string tableGroup = column.TableGroup ?? "";
                if (!result.Contains(tableGroup))
                {
                    result.Add(tableGroup);
                }
            }
            return result;
        }

        private static string InParameter(int count)
        {
            if (count == 0)
                return " IN()";
            if (count == 1)
                return "= @p0";

            StringBuilder sb = new StringBuilder();
            sb.Append(" IN(");
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append("@p").Append(i);
            }
            sb.Append(')');
            return sb.ToString();
        }
        private static SqlTask GetSqlBuilder(Dictionary<string, SqlTask> dictGroupSql, string tableGroup, SqlTask headSqlBuilderTupe)
        {
            if (string.IsNullOrEmpty(tableGroup))
            {
                return headSqlBuilderTupe;
            }
            return dictGroupSql[tableGroup];
        }
    
    private static IColumnValuePair[] TryAddColumnValuePair(IColumnValuePair[] inputValues, IColumnValuePair addpair)
        {
            int addColumnIndex = addpair.Column.ColumnIndex;

            // 检查是否已经存在相同的列索引
            for (int i = 0; i < inputValues.Length; i++)
            {
                if (inputValues[i].Column.ColumnIndex == addColumnIndex)
                {
                    return inputValues;
                }
            }

            // 如果不存在，则创建一个新的数组并将新列值对添加到数组末尾
            IColumnValuePair[] newArray = new IColumnValuePair[inputValues.Length + 1];
            Array.Copy(inputValues, newArray, inputValues.Length);
            newArray[newArray.Length - 1] = addpair;

            return newArray;
        }


        public static TableColumnConverterContainer? GetTableConverter(DbMetadataTable table)
        {
            return CreateTableConverter(table);

        }
        // 创建表转换器
        public static TableColumnConverterContainer CreateTableConverter(DbMetadataTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            DbMetadataColumnCollection columns = table.Columns;
            Func<object, object>[] converters = new Func<object, object>[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                converters[i] = GetConverter(columns[i]);
            }

            return new TableColumnConverterContainer(converters);
        }



        public static string GetTableNameWithGroup(string tableName, string tableGroup)
        {
            if (tableGroup.IsNullOrWhiteSpace())
            {
                return tableName;
            }
            return tableName + "_" + tableGroup;
        }


        // 获取列转换器
        private static Func<object, object> GetConverter(DbMetadataColumn col)
        {
            //if (col.Encrypt)
            //{
            //    return o => Encode(o);
            //}

            if (col.ClrType == typeof(DateTime))
            {
                if (col.DbType == 91)
                {
                    return o =>o;
                }
            }
            else if (col.ClrType == typeof(bool))
            {
                if (col.DbType == 1)
                {
                    return o =>
                    {
                        bool v = (bool)o;
                        return v ? "1" : "0";
                    };
                }
            }
            else if (col.ClrType == typeof(string) && col.DbType == 1)
            {
                return o =>
                {
                    string v = (string)o;
                    return string.IsNullOrEmpty(v) ? " " : v;
                };
            }

            return null;
        }


        // 获取表的层级
        public static int GetTableLevel(DbMetadataTable currentTable)
        {
            if (currentTable == null)
                throw new ArgumentNullException(nameof(currentTable));

            int level = 1;
            DbMetadataRelation parentRelation = currentTable.ParentRelation;

            while (parentRelation != null)
            {
                DbMetadataTable childTable = parentRelation.ParentTable;
                parentRelation = childTable.ParentRelation;
                level++;
            }

            return level;
        }

        public static bool GetExTableHaveRelitionField()
        {
            return true;
        }
    }
}
