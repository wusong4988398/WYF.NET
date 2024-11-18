using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.DataEntity;
using WYF.DataEntity;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.DbEngine;
using WYF.DbEngine.db;
using WYF.Form.DataEntity;
using WYF.OrmEngine.dataManager;
using WYF.OrmEngine.Drivers;
using WYF.OrmEngine.Impl;
using WYF.OrmEngine.Query.Crud;


namespace WYF.OrmEngine.DataEntity
{
    public class DataManagerImplement : IDataManager
    {
        private static  Object[] EmptyObjectArray = new Object[0];

        private int batchDeleteSize = 10000;
        private OperateOption _option;
        private int maxObjects = 1000000;

        public bool IsSelectHeadOnly { get; set; } = false;
        private TableAliasGenner aliasGenner = new TableAliasGenner();
        private static readonly Object[] EmptyDynmaicObjects = new DynamicObject[0];
        private static readonly Object[] EmptyObjects = new Object[0];

        private static  IColumnValuePair[] EmptyColumnValuePairArray;

        public DataEntityTypeMap DataEntityTypeMap { get; set; }
        private Dictionary<DbMetadataTable, EntryInfo> entryPageInfo = null;
        //public DbMetadataDatabase DataDatabase { get; set; }
        private int? pageSize;
        private int startRowIndex;
        private DBRoute _dbRoute = null;
        private IDataEntityType _dataEntityType;
        private ObjectCache<IDataEntityType, Tuple<DataEntityTypeMap, DbMetadataDatabase>> _cache;
        private SaveDataSet saveDataSet;
        static DataManagerImplement()
        {
            EmptyColumnValuePairArray=new IColumnValuePair[0];
        }
        public DataManagerImplement()
        {
            this._cache = ObjectCache<IDataEntityType, Tuple<DataEntityTypeMap, DbMetadataDatabase>>.Create();
            Init();
        }

        public DataManagerImplement(IDataEntityType dt, DBRoute dbRoute)
        {
           
            this._cache = ObjectCache<IDataEntityType, Tuple<DataEntityTypeMap, DbMetadataDatabase>>.Create();
            this._dbRoute = dbRoute;
            SetDataEntityType(dt);
            Init();
        }
        public DataManagerImplement(IDataEntityType dt) : this(dt, null)
        {
            Console.WriteLine("2323");
        }
  

        public object[] Read(object[] ids)
        {
            if (ids.Length == 0)
            {
                if (this._dataEntityType is DynamicObjectType)
                    return EmptyDynmaicObjects;
                return EmptyObjects;
            }
            ReadWhere where = new ReadWhere(ids);
            Object[] objs = Read(where);
            return objs;
        }

        public object[] Read(ReadWhere where)
        {

            QuickDataSet dataSet = ReadToDataSet(this.Database, this.DataEntityTypeMap.DbTable, where);
            object[] entities = DataSetToEntities(dataSet);
            SetEntitySnapshot(entities, dataSet);
            return entities;
        }

        private void SetEntitySnapshot(Object[] entities, QuickDataSet dataSet)
        {
            if (entities.Length == 0)
                return;
            if (entities.Length == 1)
            {
                SetSingleEntitySnapshot(entities, dataSet);
            }
            else
            {
                SetMoreEntitySnapshot(entities, dataSet);
            }
        }
        private void Init()
        {
            try
            {
                this.batchDeleteSize = 10000;
                this.maxObjects = 1000000;
                if (this.batchDeleteSize < 0)
                    this.batchDeleteSize = 10000;
                if (this.maxObjects < 0)
                    this.maxObjects = 1000000;
            }
            catch (Exception e)
            {
            }
        }
        private void SetMoreEntitySnapshot(Object[] entities, QuickDataSet dataSet)
        {
            IDataEntityType dt = this.DataEntityTypeMap.DataEntityType;
            string rootTableName = this.DataEntityTypeMap.DbTable.Name;
            dataSet.Tables.TryGetValue(rootTableName, out QuickDataTable rootTable);
            QuickRow[] rows = rootTable.Rows;
            int rowCount = rows.Length;
            PkSnapshot pkSnap = null;
            RuntimePkSnapshotSet[] runtimeSets = new RuntimePkSnapshotSet[rowCount];
            int tableCount = dataSet.Tables.Count;
            Dictionary<Object, RuntimePkSnapshotSet> dict = new Dictionary<object, RuntimePkSnapshotSet>();
            int primaryColumnIndex = rootTable.Schema.PrimaryKey.ColumnIndex;
            int rowIndex;
            for (rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                RuntimePkSnapshotSet runtimeSet = new RuntimePkSnapshotSet();
                runtimeSet.PkSnapshotSet = new PkSnapshotSet(tableCount);
                runtimeSet.Tables = new RuntimePkSnapshot[tableCount];
                List<PkSnapshot> snapshots = runtimeSet.PkSnapshotSet.Snapshots;
                for (int j = 0; j < tableCount; j++)
                {
                    PkSnapshot tempVar = new PkSnapshot();
                    tempVar.TableName = ((QuickDataTable)dataSet.Tables[j]).Schema.Name;
                    pkSnap = tempVar;
                    snapshots.Add(pkSnap);
                    runtimeSet.Tables[j] = new RuntimePkSnapshot(pkSnap);
                }
                Object pkOid = (rows[rowIndex]).Values[primaryColumnIndex];
                (runtimeSet.Tables[0]).Oids.Add(pkOid);
                try
                {
                    dict.Add(pkOid, runtimeSet);
                }

                catch (ArgumentException e)
                {
                    throw new Exception(
                        String.Format("表{0}中读取出的数据，出现重复的主键({1})数据:{2}", rootTableName, rootTable.Schema.PrimaryKey.Name, pkOid), e);
                }
                runtimeSets[rowIndex] = runtimeSet;
            }
            foreach (DbMetadataTable childTable in rootTable.Schema.ChildTables)
                SetEntitySnapshotEx(dataSet, (QuickDataTable)dataSet.Tables[childTable.Name], dict);
            for (rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                runtimeSets[rowIndex].Complete();
                dt.SetPkSnapshot(entities[rowIndex], (runtimeSets[rowIndex]).PkSnapshotSet);
            }
        }

        private void SetEntitySnapshotEx(QuickDataSet dataSet, QuickDataTable currentTable, Dictionary<Object, RuntimePkSnapshotSet> pkDict)
        {

            int tableIndex = dataSet.FindIndex(currentTable);
            DbMetadataTable tableSchema = currentTable.Schema;
            int parentColumnId = tableSchema.ParentRelation.ChildColumn.ColumnIndex;
            int pkColumnId = tableSchema.PrimaryKey.ColumnIndex;
            DbMetadataTableCollection childTables = tableSchema.ChildTables;
            Dictionary<Object, RuntimePkSnapshotSet> newPkDict = null;
            if (childTables.Count() > 0)
                newPkDict = new Dictionary<object, RuntimePkSnapshotSet>();
            foreach (QuickRow row in currentTable.Rows)
            {
                Object parentOid = row.Values[parentColumnId];
                Object pkOid = row.Values[pkColumnId];
                RuntimePkSnapshotSet snapshotSet = pkDict.GetValueOrDefault(parentOid, null);
                if (snapshotSet != null)
                {
                    (snapshotSet.Tables[tableIndex]).Oids.Add(pkOid);
                    if (newPkDict != null)
                        try
                        {
                            newPkDict.Add(pkOid, snapshotSet);
                        }
                        catch (ArgumentException e)
                        {
                            throw new Exception(
                                 String.Format("表{0}中读取出的数据，出现重复的主键({1})数据:{2}", currentTable.Schema.Name, currentTable.Schema.PrimaryKey.Name, pkOid), e);
                        }
                }
            }
            if (this.entryPageInfo != null && currentTable.Schema.IsSubEntry)
            {
                EntryInfo entryInfo = this.entryPageInfo.GetValueOrDefault(currentTable.Schema);
                if (entryInfo != null)
                    foreach (QuickRow row in currentTable.Rows)
                    {
                        Object parentOid = row.Values[parentColumnId];
                        RuntimePkSnapshotSet snapshotSet = pkDict.GetValueOrDefault(parentOid);
                        RuntimePkSnapshot snapshot = snapshotSet.Tables[tableIndex];
                        if (snapshot.Oids.Count >= entryInfo.PageSize)
                            snapshot.ParentIds.Add(row.Values[parentColumnId]);
                    }
            }
            if (newPkDict != null)
                foreach (DbMetadataTable childTable in childTables)
                    SetEntitySnapshotEx(dataSet, (QuickDataTable)dataSet.Tables[childTable.Name], newPkDict);
        }

        private void SetSingleEntitySnapshot(Object[] entities, QuickDataSet dataSet)
        {
            PkSnapshotSet pkSnapshotSet = new PkSnapshotSet();
            foreach (QuickDataTable table in dataSet.Tables)
            {
                int primaryColumnIndex = table.Schema.PrimaryKey.ColumnIndex;
                QuickRow[] rows = table.Rows;
                PkSnapshot pkSnapshot = new PkSnapshot();
                pkSnapshot.TableName = table.Schema.Name;
                pkSnapshot.Oids = new Object[rows.Length];
                for (int j = 0; j < table.Rows.Length; j++)
                    pkSnapshot.Oids[j] = (rows[j]).Values[primaryColumnIndex];
                if (this.entryPageInfo != null && table.Schema.IsSubEntry)
                {
                    EntryInfo entryInfo = this.entryPageInfo.GetValueOrDefault(table.Schema);
                    if (entryInfo != null &&
                      pkSnapshot.Oids.Length >= entryInfo.PageSize)
                    {
                        pkSnapshot.Opids = new Object[rows.Length];
                        int parentColumnIndex = table.Schema.ParentRelation.ChildColumn.ColumnIndex;
                        for (int i = 0; i < table.Rows.Length; i++)
                            pkSnapshot.Opids[i] = (rows[i]).Values[parentColumnIndex];
                    }
                }
                pkSnapshotSet.Snapshots.Add(pkSnapshot);
            }
            IDataEntityType dt = this.DataEntityTypeMap.DataEntityType;
            dt.SetPkSnapshot(entities[0], pkSnapshotSet);
        }


        private Object[] DataSetToEntities(QuickDataSet dataSet)
        {
            Object[] entities = null;
            string rootTableName = this.DataEntityTypeMap.DbTable.Name;
            QuickDataTable rootTable = (QuickDataTable)dataSet.Tables[rootTableName];
            return DataSetToEntities(entities, rootTable, dataSet, this.DataEntityTypeMap);
        }
        private Object[] DataSetToEntities(Object[] entities, QuickDataTable table, QuickDataSet dataSet, DataEntityTypeMap dtMap)
        {
            QuickRow[] rows = table.Rows;
            bool isSupportInitialize = false;
            IDataEntityType dt = dtMap.DataEntityType;
            if (entities == null)
            {
                if (dt is DynamicObjectType)
                {
                    entities = new DynamicObject[rows.Length];
                }
                else
                {
                    entities = new Object[rows.Length];
                }
                for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                    entities[rowIndex] = dt.CreateInstance();
            }
            if (entities.Length == 0)
                return entities;
            bool isFirstData = true;
            for (int i = 0; i < entities.Length; i++)
            {
                Object dataEntity = entities[i];
                if (dataEntity != null)
                {
                    if (isFirstData)
                    {
                        if (dataEntity is ISupportInitialize)
                        {
                            isSupportInitialize = true;
                            isFirstData = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                     ((ISupportInitialize)dataEntity).BeginInit();
                }
            }
            foreach (SimplePropertyMap spMap in dtMap.SimpleProperties)
                SetSimplePropertyValues(entities, dtMap, rows, spMap);
            foreach (CollectionPropertyMap colMap in dtMap.CollectionProperties)
                SetCollectionPropertyValues(entities, table, dataSet, colMap);

            if (isSupportInitialize)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Object dataEntity = entities[i];
                    if (dataEntity != null)
                    {
                        ((ISupportInitialize)dataEntity).EndInit();
                        dt.SetFromDatabase(dataEntity);
                    }
                }
            }
            else
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Object dataEntity = entities[i];
                    if (dataEntity != null)
                        dt.SetFromDatabase(dataEntity);
                }
            }
            return entities;
        }

        private void SetCollectionPropertyValues(Object[] entities, QuickDataTable table, QuickDataSet dataSet, CollectionPropertyMap colMap)
        {
            Object[] newEntities = null;
            DataEntityTypeMap newDtMap = colMap.CollectionItemPropertyTypeMap;
            QuickDataTable newTable = (QuickDataTable)dataSet.Tables[newDtMap.DbTable.Name];
            newEntities = DataSetToEntities(newEntities, newTable, dataSet, newDtMap);
            ICollectionProperty colp = (ICollectionProperty)colMap.DataEntityProperty;
            bool isSupportInitialize = false;
            bool isFirstData = true;
            ISupportInitialize[] itemsList = null;
            for (int i = 0; i < entities.Length; i++)
            {
                Object dataEntity = entities[i];
                if (dataEntity != null)
                {
                    Object items = colp.GetValueFast(dataEntity);
                    if (isFirstData)
                    {
                        ISupportInitialize iSupportInitialize = (items is ISupportInitialize) ? (ISupportInitialize)items : null;
                        if (iSupportInitialize != null)
                        {
                            isSupportInitialize = true;
                            itemsList = new ISupportInitialize[entities.Length];
                            isFirstData = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                    ISupportInitialize spObject = (ISupportInitialize)items;
                    spObject.BeginInit();
                    itemsList[i] = spObject;
                }
            }
            IDataEntityType newDt = colp.ItemType;
            int parentColumnIndex = colMap.ParentColumn.ColumnIndex;
            QuickRow[] newRows = newTable.Rows;
            Dictionary<Object, int> rowCountMap = newTable.EntryRowCount;
            if (rowCountMap != null)
                foreach (var rowCount in rowCountMap)
                {
                    int? parentRowIndex = table.GetRowIndexByParmaryKey(rowCount.Key);
                    if (parentRowIndex != null)
                    {
                        DataEntityBase parentObj = (DataEntityBase)entities[parentRowIndex.Value];

                        parentObj.DataEntityState.SetEntryRowCount(newDt.Name, rowCount.Value);
                    }
                }
            if (this.entryPageInfo != null)
            {
                EntryInfo info = this.entryPageInfo.GetValueOrDefault(colMap.CollectionItemPropertyTypeMap.DbTable);
                if (info != null)
                    for (int k = 0; k < entities.Length; k++)
                    {
                        DataEntityBase parentObj = (DataEntityBase)entities[k];
                        parentObj.DataEntityState.SetEntryPageSize(newDt.Name, info.PageSize);
                        parentObj.DataEntityState.SetEntryStartRowIndex(newDt.Name, info.StartRowIndex);
                    }
            }

            Object lastParentPKValue = null;
            List<Object> lastParentList = null;

            for (int j = 0; j < newEntities.Length; j++)
            {
                Object parentPKValue = (newRows[j]).Values[parentColumnIndex];
                if (!parentPKValue.Equals(lastParentPKValue))
                {
                    int? parentRowIndex = table.GetRowIndexByParmaryKey(parentPKValue);
                    if (parentRowIndex != null)
                    {
                        lastParentList = (List<Object>)colp.GetValue(entities[parentRowIndex.Value]);
                        lastParentPKValue = parentPKValue;
                    }
                    else
                    {
                        continue;
                    }
                }
                lastParentList.Add(newEntities[j]);
                continue;
            }
            if (isSupportInitialize)
                for (int j = 0; j < itemsList.Length; j++)
                {
                    ISupportInitialize item = itemsList[j];
                    if (item != null)
                        item.EndInit();
                }
        }

        private void SetSimplePropertyValues(Object[] entities, DataEntityTypeMap dtMap, QuickRow[] rows, SimplePropertyMap spMap)
        {
            ISimpleProperty sp = spMap.DataEntityProperty;
            int columnIndex = spMap.DbColumn.ColumnIndex;
            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                QuickRow row = rows[rowIndex];
                Object entity = entities[rowIndex];
                Object value = row.Values[columnIndex];
                if (entity != null && value != null)
                    sp.SetValueFast(entity, value);

            }
        }

        private QuickDataSet ReadToDataSet(DbMetadataDatabase database, DbMetadataTable rootTable, ReadWhere where)
        {
            DbMetadataTableCollection tables = database.Tables;
            if (this.pageSize != null)
            {
                this.entryPageInfo = new Dictionary<DbMetadataTable, EntryInfo>();
                foreach (DbMetadataTable tableSchema in tables)
                {
                    if (this.pageSize != null && tableSchema.Seq != null)
                    {
                        int startRow = this.startRowIndex;
                        if (tableSchema.ParentRelation != null && tableSchema
                          .ParentRelation.ParentTable != rootTable)
                            startRow = 0;
                        CreateEntryPageInfo(startRow, this.pageSize.Value, tableSchema);
                    }
                }
            }
            return Select(tables, rootTable, where);
        }

        public QuickDataSet Select(Collection<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, ReadWhere where)
        {
            Collection<DbMetadataTable> tables = (Collection<DbMetadataTable>)tablesSchema;
            List<SelectSql> selectSQLAll = new List<SelectSql>(tables.Count);
            SqlBuilder whereSql = GetRootTableWhereSQl(rootTable, where);
            foreach (DbMetadataTable tableSchema in tablesSchema)
            {
                DbMetadataTable stopJoinTable = null;
                if (tableSchema != rootTable)
                {
                    SqlBuilder whereSqlChild = GetChildTableWhereSQl(tableSchema, rootTable, where, whereSql);
                    if (whereSqlChild != whereSql)
                    {
                        stopJoinTable = rootTable;
                        whereSql = whereSqlChild;
                    }
                }
                SelectSql tupSQL = GetSelectSQL(tableSchema, stopJoinTable);
                SqlBuilder selectSql = new SqlBuilder();
                selectSql.Append("/*ORM*/ ", new Object[0]).Append(tupSQL.selectSql, new Object[0]);
                if (!whereSql.IsEmpty)
                {
                    selectSql.Append(" WHERE ", new Object[0]);
                    selectSql.AppendSqlBuilder(whereSql);
                    if (tupSQL.SelectWhere.Length > 0)
                        selectSql.Append(" AND ", new Object[0]).Append(tupSQL.SelectWhere, (tupSQL.SelectParams != null) ? tupSQL.SelectParams.ToArray() : null);
                }
                else if (tupSQL.SelectWhere.Length > 0)
                {
                    selectSql.Append(" WHERE ", new Object[0]).Append(tupSQL.SelectWhere, (tupSQL.SelectParams != null) ? tupSQL.SelectParams.ToArray() : null);
                }
                String sqlCount = tupSQL.CountSql;
                SqlBuilder countBuild = new SqlBuilder();
                if (sqlCount.Length > 0 && !whereSql.IsEmpty)
                {
                    sqlCount = String.Format(sqlCount, "WHERE");
                    countBuild.Append("/*ORM*/ ").Append(sqlCount).AppendSqlBuilder(whereSql)
                      .Append((tupSQL.CountWhere.Length > 0) ? (" AND " + tupSQL.CountWhere) : " ", (tupSQL.CountParams != null) ? tupSQL.CountParams
                        .ToArray() : null)
                      .Append(!tupSQL.CountGroupBySqlpart.IsNullOrEmpty() ? " " : tupSQL.CountGroupBySqlpart, new Object[0]);
                }
                selectSql.Append(GetSortSQL(tableSchema), new Object[0]);
                tupSQL.SelectSqlBuild = selectSql;
                tupSQL.CountSqlBuild = countBuild;
                selectSQLAll.Add(tupSQL);

            }
            return ExecuteReader(tablesSchema, rootTable, selectSQLAll, where);

        }

        protected QuickDataSet ExecuteReader(Collection<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, List<SelectSql> selectSqls, ReadWhere where)
        {
            
            int idx = 0;
            QuickDataSet ds = new QuickDataSet();
            foreach (DbMetadataTable tableSchema in tablesSchema)
            {
                QuickDataTable table = new QuickDataTable(tableSchema);
                if (this.IsSelectHeadOnly && tableSchema != rootTable && (
                   !tableSchema.Name.StartsWith(rootTable.Name)))
                {
                    table.Rows = new QuickRow[0];
                }
                else
                {

                    SelectSql selectInfo = selectSqls[idx];
                    List<QuickRow> rows = DoExecute(selectInfo.SelectSqlBuild, where, tableSchema, rootTable, QuickRowSetCallBack(tableSchema));
                    table.Rows = rows.ToArray();
                    if (this.entryPageInfo != null)
                    {
                        EntryInfo entryInfo = this.entryPageInfo.GetValueOrDefault(tableSchema, null);
                        if (entryInfo != null &&
                          table.Rows.Length >= entryInfo.PageSize && !selectInfo.CountSqlBuild.IsEmpty)
                            table.EntryRowCount = DoExecute(selectInfo.CountSqlBuild, where, tableSchema, rootTable, RowCountSetCallBack());
                    }
                }
                ds.Tables.Add(table);
                idx++;
            }
            return ds;

        }

        private static Func<IDataReader, Dictionary<object, int>> RowCountSetCallBack()
        {
            return (dr) =>
            {
                Dictionary<object, int> rowCount = new Dictionary<object, int>();
                while (dr.Read())
                {
                    rowCount[dr[0]] = dr[1].ToInt();
                }
                return rowCount;
            };
        }

        private static Func<IDataReader, List<QuickRow>> QuickRowSetCallBack(DbMetadataTable tableSchema)
        {
            return (reader) =>
            {

                List<QuickRow> rows = new List<QuickRow>();
                while (reader.Read())
                {
                    QuickRow row = new QuickRow();
                    int columnCount = tableSchema.Columns.Count;
                    row.Values = new object[columnCount];
                    for (int i = 0; i < tableSchema.Columns.Count; i++)
                    {
                        DbMetadataColumn column = tableSchema.Columns[i];
                        row.Values[i] = reader[i];
                    }

                    rows.Add(row);

                }
                return rows;
            };
        }


        private T DoExecute<T>(SqlBuilder builder, ReadWhere where, DbMetadataTable tableSchema, DbMetadataTable rootTable, Func<IDataReader, T> action)
        {
            
            return DB.Query<T>(this._dbRoute, builder, action);
        }
        protected String GetSortSQL(DbMetadataTable tableSchema)
        {
            if (tableSchema.SortColumns == null)
                return " ";
            if (tableSchema.SortColumns.Count == 0)
                return " ";
            StringBuilder sb = new StringBuilder(" ORDER BY ");
            foreach (String field in tableSchema.SortColumns)
                sb.Append(GetColumnNameSql(tableSchema.Name, field)).Append(',');

            return sb.SubString(0, sb.Length - 1);
        }
        protected SelectSql GetSelectSQL(DbMetadataTable tableSchema, DbMetadataTable stopJoinTable)
        {
            return GetSelectSQL(tableSchema, false, stopJoinTable);
        }
        public SelectSql GetSelectSQL(DbMetadataTable currentTable, bool isAccess, DbMetadataTable stopJoinTable)
        {
            string sqlFrom = GetTableNameSql(currentTable.Name, true);
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlJoinWhere = new StringBuilder();
            SqlBuilder builder = new SqlBuilder();
            List<SqlParam> parameters = new List<SqlParam>();
            List<SqlParam> Joinparameters = new List<SqlParam>();
            DbMetadataRelation parentRelation = currentTable.ParentRelation;
            DbMetadataTable childTable = currentTable;
            String parentField = null;
            if (this.entryPageInfo != null)
            {
                DbMetadataColumn seq = currentTable.Seq;
                if (seq != null)
                {
                    EntryInfo pageInfo = this.entryPageInfo.GetValueOrDefault(currentTable);
                    if (pageInfo.StartRowIndex == 0)
                    {
                        parentField = GetColumnNameSql(childTable.Name, parentRelation.ChildColumn.Name);
                        sqlCount.Append(" SELECT ").Append(parentField).Append(", Count(*)   FROM ");
                    }
                    sqlWhere.Append(GetColumnNameSql(currentTable.Name, seq.Name)).Append(" BETWEEN ? AND ? ");
                    parameters.Add(new SqlParam(KDbType.Int32, pageInfo.StartRowIndex + 1));
                    parameters.Add(new SqlParam(KDbType.Int32, pageInfo.StartRowIndex + pageInfo.PageSize));
                }
            }
            StringBuilder sbFrom = new StringBuilder(sqlFrom);
            while (parentRelation != null)
            {
                DbMetadataTable parentTable = parentRelation.ParentTable;
                if (parentTable != stopJoinTable)
                    sbFrom.Insert(0, isAccess ? "(" : " ").Append(" INNER JOIN ")
                      .Append(GetTableNameSql(parentTable.Name, true)).Append(" ON ")
                      .Append(GetColumnNameSql(childTable.Name, parentRelation.ChildColumn.Name))
                      .Append(" = ")
                      .Append(GetColumnNameSql(parentTable.Name, parentTable.PrimaryKey.Name))
                      .Append(isAccess ? ")" : " ");
                if (this.entryPageInfo != null)
                {
                    DbMetadataColumn seq = parentTable.Seq;
                    if (seq != null)
                    {
                        EntryInfo pageInfo = this.entryPageInfo.GetValueOrDefault(parentTable);
                        if (sqlJoinWhere.Length > 0)
                            sqlJoinWhere.Append(" AND ");
                        sqlJoinWhere.Append(GetColumnNameSql(parentTable.Name, seq.Name))
                          .Append(" BETWEEN ? AND ? ");
                        Joinparameters.Add(new SqlParam(KDbType.Int32, pageInfo.StartRowIndex + 1));
                        Joinparameters
                          .Add(new SqlParam(KDbType.Int32, pageInfo.StartRowIndex + pageInfo.PageSize));
                    }
                }
                childTable = parentRelation.ParentTable;
                parentRelation = childTable.ParentRelation;
            }
            List<String> tableGroups = GetTableGroups(currentTable);
            if (tableGroups.Count > 0)
            {
                String temp = " ON " + GetColumnNameSql(currentTable.Name, currentTable.PrimaryKey.Name) + " = ";
                foreach (String tableGroup in tableGroups)
                {
                    String exTableName = currentTable.Name + "_" + tableGroup;
                    sbFrom.Insert(0, isAccess ? "(" : " ")
                        .Append(" LEFT JOIN ").Append(GetTableNameSql(exTableName, true))
                      .Append(temp).Append(GetColumnNameSql(exTableName, currentTable.PrimaryKey.Name))
                      .Append(isAccess ? ")" : " ");
                }
            }
            StringBuilder sqlColumns = new StringBuilder(currentTable.Columns.Count * 40);
            sqlColumns.Append("SELECT ");
            bool isFirst = true;
            foreach (DbMetadataColumn column in currentTable.Columns)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sqlColumns.Append(',');
                }
                sqlColumns.Append(GetColumnNameSql(currentTable, column));
            }
            sqlColumns.Append(" FROM ");
            sqlColumns.Append(sbFrom);
            SelectSql selectSql = new SelectSql();
            if (parentField != null)
            {
                sqlCount.Append(sbFrom).Append(" {0} ");
                selectSql.CountGroupBySqlpart = " GROUP BY " + parentField + " ";
            }
            selectSql.selectSql = sqlColumns.ToString();
            if (sqlJoinWhere.Length > 0)
                if (sqlWhere.Length > 0)
                {
                    sqlWhere.Append(" AND ").Append(sqlJoinWhere);
                }
                else
                {
                    sqlWhere.Append(sqlJoinWhere);
                }
            selectSql.SelectWhere = sqlWhere.ToString();
            parameters.AddRange(Joinparameters);
            selectSql.SelectParams = parameters;
            selectSql.CountSql = sqlCount.ToString();
            selectSql.CountWhere = sqlJoinWhere.ToString();
            selectSql.CountParams = Joinparameters;
            return selectSql;

        }

        private List<string> GetTableGroups(DbMetadataTable currentTable)
        {
            List<String> result = new List<String>();
            foreach (DbMetadataColumn column in currentTable.Columns)
            {
      

                if (!column.TableGroup.IsNullOrEmpty() &&
                  !result.Contains(column.TableGroup))
                    result.Add(column.TableGroup);
            }
            return result;
        }
        public String GetTableNameSql(String name, bool withAlias)
        {
            return this.aliasGenner.GetTableName(name, withAlias);
        }
        private SqlBuilder GetChildTableWhereSQl(DbMetadataTable childTable, DbMetadataTable rootTable, ReadWhere where, SqlBuilder rootwhereSql)
        {
     
            if (!string.IsNullOrEmpty(where.WhereSql))
                return rootwhereSql;
            SqlBuilder whereSql = new SqlBuilder();
            DbMetadataTable table = childTable;
            while (rootTable != table.ParentRelation.ParentTable)
                table = table.ParentRelation.ParentTable;
            String pk = GetColumnNameSql(table.Name, table.ParentRelation.ChildColumn.Name);
            if (where.IsSingleValue)
            {
                SqlParam pkValueParameter = new SqlParam("@PKValue", where.ReadOids[0], (KDbType)rootTable.PrimaryKey.DbType);
                whereSql.Append(pk + "= ?", new Object[] { pkValueParameter });
            }
            else if (where.ReadOids != null)
            {
                whereSql.AppendSqlBuilder(
                    GetIdsWhereSql(table.ParentRelation.ChildColumn.DbType, pk, where));
            }
            return whereSql;

        }

        private SqlBuilder GetRootTableWhereSQl(DbMetadataTable rootTable, ReadWhere where)
        {
            SqlBuilder build = new SqlBuilder();
            string pk = GetColumnNameSql(rootTable.Name, rootTable.PrimaryKey.Name);
            if (where.IsSingleValue)
            {
                SqlParam pkValueParameter = new SqlParam("@PKValue", where.ReadOids[0],(KDbType)rootTable.PrimaryKey.DbType);
                build.Append(pk + "= @PKValue", pkValueParameter);
                
            }
            else if (where.ReadOids != null)
            {
                build.AppendSqlBuilder(GetIdsWhereSql(rootTable.PrimaryKey.DbType, pk, where));
            }
  
            if (!string.IsNullOrEmpty(where.WhereSql))
            {
                if (!build.IsEmpty)
                    build.Append(" AND ");
                build.Append(where.WhereSql, where.SqlParams?.ToArray());
            }
            return build;
        }

        private SqlBuilder GetIdsWhereSql(int pkDbType, string fieldName, ReadWhere where)
        {
            object[] ids = where.ReadOids;
            SqlBuilder build = new SqlBuilder();
            if (ids == null || ids.Length == 0)
            {
                build.Append(" 0=1 ", new Object[0]);
                return build;
            }
            build.AppendIn(fieldName, ids);
            return build;
        }

        public string GetColumnNameSql(string tableName, string columnName)
        {
            return this.aliasGenner.GetAlias(tableName) + '.' + columnName;
        }
        public String GetColumnNameSql(DbMetadataTable currentTable, DbMetadataColumn column)
        {
            if (string.IsNullOrEmpty(column.TableGroup))
                return GetColumnNameSql(currentTable.Name, column.Name);
            return GetColumnNameSql(currentTable.Name + '_' + column.TableGroup, column.Name);
        }

        private void CreateEntryPageInfo(int startRowIndex, int pageSize, DbMetadataTable tableSchema)
        {
            if (this.entryPageInfo == null)
                this.entryPageInfo = new Dictionary<DbMetadataTable, EntryInfo>();
            EntryInfo pageInfo = new EntryInfo();
            pageInfo.PageSize = pageSize;
            pageInfo.StartRowIndex = startRowIndex;
            this.entryPageInfo[tableSchema] = pageInfo;

        }

        public void SetDataEntityType(IDataEntityType value)
        {
            Tuple<DataEntityTypeMap, DbMetadataDatabase> temp = null;
            if (this._dbRoute == null)
                this._dbRoute = DBRoute.Of(value.DBRouteKey);
            CheckDBRoute(value);
            if (value == null)
                throw new ArgumentException("设置数据管理器实体类型失败，实体类型DataEntityType不能为空！");
            this._dataEntityType = value;
            //bool cacheMetadata = DataManagerUtils.GetCacheMetadata();
            bool cacheMetadata =false;
            if (cacheMetadata)
            {

            }
            else
            {
                DbMetadataDatabase db = null;
                DataEntityTypeMap map = DataEntityTypeMap.Build(value, ref db);
                temp = new Tuple<DataEntityTypeMap, DbMetadataDatabase>(map, db);
            }
            this.DataEntityTypeMap = temp.Item1;
            this.Database = temp.Item2;

        }
        private void CheckDBRoute(IDataEntityType dt)
        {
            if (this._dbRoute.RouteKey == null || this._dbRoute.RouteKey.Trim().Length == 0)
                throw new ArgumentException($"ORM: 元数据{dt.Name}的routeKey为空!");
        }
        private void SetEntitySnapshotSave(object[] entities, ISaveDataSet dataSet, OperateOption option = null)
        {
            if (((option == null) || !option.GetVariableValue<bool>("CommitFlag", false)) && (entities.Length != 0))
            {
                if (entities.Length == 1)
                {
                    IDataEntityType dataEntityType = this.DataEntityTypeMap.DataEntityType;
                    PkSnapshotSet pkSnapshotSet = new PkSnapshotSet(dataSet.Tables.Count);
                    IList<PkSnapshot> snapshots = pkSnapshotSet.Snapshots;
                    object[] objArray = null;
                    foreach (ISaveDataTable table in dataSet.Tables)
                    {
                        if (table.SaveRows.Length > 0)
                        {
                            objArray = new object[table.SaveRows.Length];
                            for (int i = 0; i < table.SaveRows.Length; i++)
                            {
                                objArray[i] = table.SaveRows[i].Oid.Value;
                            }
                        }
                        else
                        {
                            objArray = null;
                        }
                        PkSnapshot item = new PkSnapshot
                        {
                            TableName = table.Schema.Name,
                            Oids = objArray
                        };
                        snapshots.Add(item);
                    }
                    dataEntityType.SetPkSnapshot(entities[0], pkSnapshotSet);
                }
                else
                {
                    RuntimePkSnapshotSet set2;
                    IDataEntityType type2 = this.DataEntityTypeMap.DataEntityType;
                    string name = this.DataEntityTypeMap.DbTable.Name;
                    ISaveDataTable table2 = dataSet.Tables[name];
                    ISaveMetaRow[] saveRows = table2.SaveRows;
                    PkSnapshot snapshot2 = null;
                    RuntimePkSnapshotSet[] setArray = new RuntimePkSnapshotSet[saveRows.Length];
                    int count = dataSet.Tables.Count;
                    Dictionary<object, RuntimePkSnapshotSet> pkDict = new Dictionary<object, RuntimePkSnapshotSet>();
                    for (int j = 0; j < saveRows.Length; j++)
                    {
                        set2 = new RuntimePkSnapshotSet
                        {
                            PkSnapshotSet = new PkSnapshotSet(0),
                            Tables = new RuntimePkSnapshot[count]
                        };
                        for (int m = 0; m < count; m++)
                        {
                            snapshot2 = new PkSnapshot
                            {
                                TableName = dataSet.Tables[m].Schema.Name
                            };
                            set2.PkSnapshotSet.Snapshots.Add(snapshot2);
                            set2.Tables[m] = new RuntimePkSnapshot(snapshot2);
                        }
                        object obj2 = saveRows[j].Oid.Value;
                        set2.Tables[0].Oids.Add(obj2);
                        pkDict[obj2] = set2;
                        setArray[j] = set2;
                    }
                    foreach (DbMetadataTable table3 in table2.Schema.ChildTables)
                    {
                        this.SetEntitySnapshotSaveEx(dataSet, dataSet.Tables[table3.Name], pkDict);
                    }
                    for (int k = 0; k < saveRows.Length; k++)
                    {
                        set2 = setArray[k];
                        for (int n = 0; n < count; n++)
                        {
                            set2.Tables[n].Snapshot.Oids = set2.Tables[n].Oids.ToArray();
                        }
                        type2.SetPkSnapshot(entities[k], setArray[k].PkSnapshotSet);
                    }
                }
            }
        }
        private static int FindIndexSave(ISaveDataSet dataSet, ISaveDataTable currentTable)
        {
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                if (object.ReferenceEquals(dataSet.Tables[i], currentTable))
                {
                    return i;
                }
            }
            throw new ORMDesignException("002032030001576", string.Format("ORM引擎保存实体时,从表结构中查找表[{0}]失败，表[{0}]不存在！", currentTable.Schema.Name));
        }
        private void SetEntitySnapshotSaveEx(ISaveDataSet dataSet, ISaveDataTable currentTable, Dictionary<object, RuntimePkSnapshotSet> pkDict)
        {
            int index = FindIndexSave(dataSet, currentTable);
            Dictionary<object, RuntimePkSnapshotSet> dictionary = new Dictionary<object, RuntimePkSnapshotSet>(currentTable.SaveRows.Length);
            foreach (ISaveMetaRow row in currentTable.SaveRows)
            {
                object obj2 = row.ParentOid.Value;
                object item = row.Oid.Value;
                RuntimePkSnapshotSet set = pkDict[obj2];
                set.Tables[index].Oids.Add(item);
                dictionary[item] = set;
            }
            foreach (DbMetadataTable table in currentTable.Schema.ChildTables)
            {
                this.SetEntitySnapshotSaveEx(dataSet, dataSet.Tables[table.Name], dictionary);
            }
        }

        private static void ClearEntityDirty(object[] dataEntities, DataEntityTypeMap dataEntityTypeMap, OperateOption option = null)
        {
            if ((option == null) || !option.GetVariableValue<bool>("CommitFlag", false))
            {
                object obj2;
                IDataEntityType dataEntityType = dataEntityTypeMap.DataEntityType;
                for (int i = 0; i < dataEntities.Length; i++)
                {
                    if (dataEntities[i] != null)
                    {
                        obj2 = dataEntities[i];
                        dataEntityType.SetFromDatabase(obj2);
                    }
                }
                foreach (ComplexPropertyMap map in dataEntityTypeMap.ComplexProperties)
                {
                    object[] objArray = new object[dataEntities.Length];
                    IComplexProperty dataEntityProperty = map.DataEntityProperty;
                    for (int j = 0; j < dataEntities.Length; j++)
                    {
                        obj2 = dataEntities[j];
                        if (obj2 != null)
                        {
                            objArray[j] = dataEntityProperty.GetValueFast(obj2);
                        }
                    }
                    ClearEntityDirty(objArray, map.ComplexPropertyTypeMap, option);
                }
                foreach (CollectionPropertyMap map2 in dataEntityTypeMap.CollectionProperties)
                {
                    ForWriteList<object> list = new ForWriteList<object>();
                    ICollectionProperty property2 = map2.DataEntityProperty;
                    for (int k = 0; k < dataEntities.Length; k++)
                    {
                        obj2 = dataEntities[k];
                        if (obj2 != null)
                        {
                            IEnumerable valueFast = (IEnumerable)property2.GetValueFast(obj2);
                            if (valueFast != null)
                            {
                                if (valueFast is DynamicObjectCollection)
                                {
                                    ((DynamicObjectCollection)valueFast).DeleteRows.Clear();
                                }
                                list.AddRange(valueFast);
                            }
                        }
                    }
                    ClearEntityDirty(list.ToArray(), map2.CollectionItemPropertyTypeMap, option);
                }
            }
        }
        public void Save(object[] dataEntity, IOrmTransaction ormTransaction = null, OperateOption option = null)
        {
            if (dataEntity == null)
            {
                throw new ORMArgInvalidException("002032030001567", "ORM引擎保存实体失败，实体不能为空！");
            }
            this.DoItInTransaction((tran, optionPrivate) => this.SavePrivate(dataEntity, tran, optionPrivate), ormTransaction, option);
        }

        private void DoItInTransaction(Action<IOrmTransaction, OperateOption> action, IOrmTransaction ormTransaction, OperateOption option)
        {
            option = this.GetPrivateOption(option);
            bool flag = false;
            try
            {
                if (ormTransaction == null)
                {
                    ormTransaction = this.DbDriver.BeginTransaction(null);
                    flag = true;
                }
                action(ormTransaction, option);
                if (flag)
                {
                    ormTransaction.Commit();
                    ormTransaction.Dispose();
                }
            }
            catch
            {
                if (flag)
                {
                    ormTransaction.Rollback();
                    ormTransaction.Dispose();
                }
                throw;
            }
        }
        private OperateOption GetPrivateOption(OperateOption option)
        {
            if (option == null)
            {
                return this._option;
            }
            return option.Merge(this._option);
        }
        public void Save(object dataEntity)
        {
            Save([dataEntity]);
        }

        public void Save(object[] dataEntities)
        {
            AutoBatchExecute(dataEntities, (idsPart) =>
            {
                return SavePrivate(idsPart);
            });
        }

        private object[] SavePrivate(object[] dataEntities, IOrmTransaction ormTransaction, OperateOption option)
        {
            SaveDataSet dataSet;
            if (dataEntities == null)
            {
                throw new ORMArgInvalidException("002032030001567", "ORM引擎保存实体失败，实体不能为空！");
            }
            if (dataEntities.Length != 0)
            {
                dataSet = new SaveDataSet();
                PkSnapshotSet pkSnapshotSet = EntitiesToSnapshot(dataEntities, this.Database, this.DataEntityTypeMap);
                EntitiesToDataSet(AddNewSaveTable(dataEntities, null, this.DataEntityTypeMap, dataSet), dataEntities, dataSet, this.DataEntityTypeMap, false);
                dataSet.AnalyseRows(pkSnapshotSet);
                this.SaveDataSet(dataSet, ormTransaction, option);
                ormTransaction.CommitAfter += delegate (object s, EventArgs e) {
                    Action action = null;
                    Action action2 = null;
                    if (dataEntities.Length > 300)
                    {
                        Action[] actions = new Action[2];
                        if (action == null)
                        {
                            action = () => ClearEntityDirty(dataEntities, this.DataEntityTypeMap, option);
                        }
                        actions[0] = action;
                        if (action2 == null)
                        {
                            action2 = () => this.SetEntitySnapshotSave(dataEntities, dataSet, option);
                        }
                        actions[1] = action2;
                        Parallel.Invoke(actions);
                    }
                    else
                    {
                        ClearEntityDirty(dataEntities, this.DataEntityTypeMap, option);
                        this.SetEntitySnapshotSave(dataEntities, dataSet, option);
                    }
                };
            }
            return EmptyObjectArray;
        }

        public object[] SavePrivate(object[] dataEntities)
        {
            if (dataEntities == null)
            {
                throw new ArgumentException("ORM: 参数不能为空");
            }
            if (dataEntities.Length == 0)
            {
                return EmptyObjectArray;
            }

            SaveDataSet dataSet = new SaveDataSet();
            PkSnapshotSet snapshotSet = EntitiesToSnapshot(dataEntities, Database, DataEntityTypeMap);
            EntitiesToDataSet(AddNewSaveTable(dataEntities, null, DataEntityTypeMap, dataSet),
                             dataEntities, dataSet, DataEntityTypeMap, false);
            dataSet.AnalyseRows(snapshotSet);
            SaveDataSet(dataSet);
            this.saveDataSet = dataSet;
            return EmptyObjectArray;
        }
        private void SaveDataSet(SaveDataSet dataSet, IOrmTransaction ormTransaction, OperateOption option)
        {
            foreach (ISaveDataTable table in dataSet.Tables)
            {
                if (table.SaveRows != null)
                {
                    for (int i = 0; i < table.SaveRows.Length; i++)
                    {
                        ISaveMetaRow row = table.SaveRows[i];
                        if (row.Operate == RowOperateType.Insert)
                        {
                            ormTransaction.Insert(table.Schema, row.DirtyValues.ToArray(), (row.OutputValues == null) ? EmptyColumnValuePairArray : row.OutputValues.ToArray(), row.Oid, option);
                        }
                        else if (row.Operate == RowOperateType.Update)
                        {
                            ormTransaction.Update(table.Schema, row.DirtyValues.ToArray(), (row.OutputValues == null) ? EmptyColumnValuePairArray : row.OutputValues.ToArray(), row.Oid, row.Version, option);
                        }
                    }
                }
                if (table.DeleteRows != null)
                {
                    object[] oids = new object[table.DeleteRows.Length];
                    for (int j = 0; j < table.DeleteRows.Length; j++)
                    {
                        oids[j] = table.DeleteRows[j].Oid;
                    }
                    ormTransaction.Delete(table.Schema, oids, null, null);
                }
            }
        }

        private void SaveDataSet(SaveDataSet dataSet)
        {
            //BeforeSaveDataSet(dataSet);
            OrmDBTasks tasks = new OrmDBTasks(true, this._dbRoute);
            foreach (ISaveDataTable table in dataSet.Tables)
            {
                if (table.SaveRows != null)
                {
                    for (int i = 0; i < table.SaveRows.Length; i++)
                    {
                        ISaveMetaRow saveRow = table.SaveRows[i];
                        if (saveRow.Operate == RowOperateType.Insert)
                        {
                            tasks.Insert(
                                table.Schema,
                                saveRow.DirtyValues.ToArray(),
                                saveRow.OutputValues?.ToArray() ?? EmptyColumnValuePairArray,
                                saveRow.Oid
                            );
                        }
                        else if (saveRow.Operate == RowOperateType.Update)
                        {
                            tasks.Update(table, saveRow);
                        }
                    }
                }
                if (table.DeleteRows != null)
                {
                    object[] oids = table.DeleteRows.Select(row => row.Oid).ToArray();
                    List<SqlTask> ts = tasks.Delete(table.Schema, oids, null);
                    //ShardingHintContext ctx = null;
                    //IColumnValuePair parentOid = ((SaveDataTable)table).GetParentOid();
                    //if (parentOid != null)
                    //{
                    //    ctx = ShardingHinter.TryHint(parentOid);
                    //}
                    //else
                    //{
                    //    ISaveDataTable rootTable = dataSet.Tables.FirstOrDefault();
                    //    ISaveMetaRow[] rootSaveRows = rootTable?.SaveRows;
                    //    if (rootSaveRows != null && rootSaveRows.Length > 0)
                    //    {
                    //        ctx = ShardingHinter.TryHint(rootTable.GetSchema().Name, rootSaveRows);
                    //    }
                    //}
                    //if (ctx != null)
                    //{
                    //    foreach (SqlTask t in ts)
                    //    {
                    //        t.SetShardingHintContext(ctx);
                    //    }
                    //}
                }
                List<Tuple<object, object, int>> changeRows = table.ChangeRows;
                if (changeRows != null && changeRows.Count > 0)
                {
                    BatchUpdateSeqTask t2 = tasks.UpdateSeq(table.Schema, changeRows);
                
                }
            }
            tasks.CommitDbTask();
        }

        private void EntitiesToDataSet(SaveDataTable table, object[] dataEntities, SaveDataSet dataSet, DataEntityTypeMap dataEntityTypeMap, bool includeDefaultValue)
        {
            foreach (ComplexPropertyMap cpxMap in dataEntityTypeMap.ComplexProperties)
            {
                object[] newDataEntities = new object[dataEntities.Length];
                IComplexProperty cpx = cpxMap.DataEntityProperty;
                for (int i = 0; i < dataEntities.Length; i++)
                {
                    object dataEntity = dataEntities[i];
                    if (dataEntity != null)
                    {
                        newDataEntities[i] = cpx.GetValueFast(dataEntity);
                        if (newDataEntities[i] != null && cpxMap.RefIdProperty != null)
                        {
                            cpxMap.RefIdProperty.SetValueFast(dataEntity, cpx.ComplexType.PrimaryKey.GetValueFast(newDataEntities[i]));
                        }
                    }
                }
                EntitiesToDataSet(AddNewSaveTable(newDataEntities, null, cpxMap.ComplexPropertyTypeMap, dataSet), newDataEntities, dataSet, cpxMap.ComplexPropertyTypeMap, includeDefaultValue);
            }

            EntitiesToDataSetForSimpleProperty(table, dataEntities, dataSet, dataEntityTypeMap, includeDefaultValue);

            foreach (CollectionPropertyMap colpMap in dataEntityTypeMap.CollectionProperties)
            {
                List<object> newEntitiesList = new List<object>(16);
                List<IColumnValuePair> parentOidList = new List<IColumnValuePair>(2);
                Dictionary<object, EntryInfo> mapEntryInfo = new Dictionary<object, EntryInfo>();
                ICollectionProperty colp = colpMap.DataEntityProperty;
                for (int i2 = 0; i2 < dataEntities.Length; i2++)
                {
                    object dataEntity2 = dataEntities[i2];
                    if (dataEntity2 != null)
                    {
                        IColumnValuePair oid = table.SaveRows[i2].Oid;
                        List<object> list = colp.GetValueFast(dataEntity2) as List<object>;
                        if (list != null)
                        {
                            EntryInfo entryInfo = colp.GetEntryInfo(dataEntity2);
                            if (entryInfo != null && entryInfo.RowCount.HasValue && entryInfo.RowCount.Value > entryInfo.PageSize)
                            {
                                entryInfo.PageSize = list.Count;
                                mapEntryInfo[oid.Value] = entryInfo;
                            }
                            newEntitiesList.AddRange(list);
                            if (dataEntities.Length == 1)
                            {
                                parentOidList.Add(oid);
                            }
                            else
                            {
                                foreach (object obj in list)
                                {
                                    parentOidList.Add(oid);
                                }
                            }
                        }
                    }
                }
                SaveDataTable newTable = AddNewSaveTable(newEntitiesList.ToArray(), parentOidList.ToArray(), colpMap.CollectionItemPropertyTypeMap, dataSet);
                if (mapEntryInfo.Count > 0)
                {
                    newTable.SetEntryInfo(mapEntryInfo);
                }
                EntitiesToDataSet(newTable, newEntitiesList.ToArray(), dataSet, colpMap.CollectionItemPropertyTypeMap, includeDefaultValue);
            }
        }
        /// <summary>
        /// 处理简单属性，将数据实体对象转换为保存数据集。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dataEntities"></param>
        /// <param name="dataSet"></param>
        /// <param name="dataEntityTypeMap"></param>
        /// <param name="includeDefaultValue"></param>
        private void EntitiesToDataSetForSimpleProperty(SaveDataTable table, object[] dataEntities, SaveDataSet dataSet, DataEntityTypeMap dataEntityTypeMap, bool includeDefaultValue)
        {
            IDataEntityType dtType = dataEntityTypeMap.DataEntityType;
            List<Tuple<DbMetadataColumn, ISimpleProperty>> outputList = GetOutputList(dataEntityTypeMap);
            ISaveMetaRow[] saveRows = table.SaveRows;

            for (int i = 0; i < dataEntities.Length; i++)
            {
                object dataEntity = dataEntities[i];
                if (dataEntity != null)
                {
                    bool hasDirtyProperty = false;
                    ForWriteList<IColumnValuePair> dirtyValues = saveRows[i].DirtyValues;
                    bool includehasDefault = includeDefaultValue;
                    bool isNewObj = false;
                    DynamicObject obj = dataEntity is DynamicObject ? (DynamicObject)dataEntity : null;
                    if (obj != null && !obj.DataEntityState.IsFromDatabase)
                    {
                        includehasDefault = true;
                        isNewObj = true;
                    }

                    List<IDataEntityProperty> listProperty = dtType.GetDirtyProperties(dataEntity, includehasDefault).ToList();
                    HashSet<string> pset = new HashSet<string>(listProperty.Select(dp => dp.Name.ToLower()));
                    //AddModifyProps(dtType, listProperty, pset, isNewObj);

                    for (int j = 0; j < listProperty.Count; j++)
                    {
                        IDataEntityProperty dp = listProperty[j];

                        bool ignore = ORMUtil.IsDbIgnoreForSave(dp);
                        if (ignore)
                        {
                            if (dp is IComplexProperty)
                            {
                                string refId = dp.Name.ToLower() + "_id";
                                if (pset.Add(refId) && dtType.Properties.ContainsKey(refId))
                                {
                                    ignore = false;
                                    object value = dp.GetValue(dataEntity);
                                    if (value is DynamicObject dynamicObj)
                                    {
                                        value = dynamicObj.PkValue;
                                    }
                                    dp = dtType.Properties[refId];
                                    dp.SetValueFast(dataEntity, value);
                                }
                            }
                        }
                        else if (dp is ISimpleProperty simpleProperty && simpleProperty.PrivacyType != 0)
                        {
                            ignore = true;

                        }

                        if (!ignore)
                        {
                            int ordinal = dp.Ordinal;
                            object propertyMap = dataEntityTypeMap.GetPropertyMapByOrdinal(ordinal);
                            if (propertyMap != null)
                            {
                                hasDirtyProperty = true;
                                //if (dp is ILocaleProperty localeProperty)
                                //{
                                //    DbMetadataColumn dbColumn = ((SimplePropertyMap)propertyMap).DbColumn;
                                //    object lc = ((ILocaleString)localeProperty.GetValueFast(dataEntity)).Get(ILocaleString.GLang);
                                //    SimpleColumnValuePair dirtyValue = new SimpleColumnValuePair(dbColumn, lc);
                                //    dirtyValues.Add(dirtyValue);
                                //}
                                //else 
                                if (dp is ISimpleProperty simpleProperty)
                                {
                                    DbMetadataColumn dbColumn2 = ((SimplePropertyMap)propertyMap).DbColumn;
                                    object v = simpleProperty.GetSaveValue(dataEntity, this.Option, saveRows[i].Operate);
                                    SimpleColumnValuePair dirtyValue2 = new SimpleColumnValuePair(dbColumn2, v);
                                    dirtyValues.Add(dirtyValue2);
                                    //if (simpleProperty.IsEncrypt && this.saveOriginalData4Encryption)
                                    //{
                                    //    string originName = dbColumn2.GetName().Substring(0, dbColumn2.GetName().LastIndexOf(EntityConst.encrypt_property_field_suffix));
                                    //    DbMetadataColumn originDbColumn = dbColumn2.Clone(originName, dbColumn2.GetEnableNull());
                                    //    originDbColumn.SetEncrypt(false);
                                    //    SimpleColumnValuePair originDirtyValue = new SimpleColumnValuePair(originDbColumn, v);
                                    //    dirtyValues.Add(originDirtyValue);
                                    //}
                                }
                                else if (dp is IComplexProperty complexProperty)
                                {
                                    object cpxEntity = dp.GetValue(dataEntity);
                                    ComplexPropertyMap cpxMap = (ComplexPropertyMap)propertyMap;
                                    if (!cpxMap.DataEntityProperty.IsReadOnly && cpxEntity != null)
                                    {
                                        cpxMap.ComplexPropertyTypeMap.DataEntityType.SetDirty(cpxEntity, true);
                                    }
                                }
                            }
                        }
                    }

               

                    if (hasDirtyProperty && outputList.Count > 0)
                    {
                        List<IColumnValuePair> list = new List<IColumnValuePair>(outputList.Count);
                        saveRows[i].OutputValues = list;

                        foreach (Tuple<DbMetadataColumn, ISimpleProperty> outputItem in outputList)
                        {
                            list.Add(new SyncColumnValuePair(outputItem.Item1, outputItem.Item2, dataEntity));
                        }
                    }
                }
            }
        }

        private List<Tuple<DbMetadataColumn, ISimpleProperty>> GetOutputList(DataEntityTypeMap dataEntityTypeMap)
        {
            List<Tuple<DbMetadataColumn, ISimpleProperty>> outputList = new List<Tuple<DbMetadataColumn, ISimpleProperty>>(dataEntityTypeMap.SimpleProperties.Count);
            foreach (SimplePropertyMap dp in dataEntityTypeMap.SimpleProperties)
            {
                if (dp.DbColumn.AutoSync != AutoSync.Never)
                {
                    outputList.Add(Tuple.Create(dp.DbColumn, dp.DataEntityProperty));
                }
            }
            return outputList;
        }
    


private SaveDataTable AddNewSaveTable(object[] dataEntities, IColumnValuePair[] parentOids, DataEntityTypeMap dataEntityTypeMap, SaveDataSet dataSet)
        {
            DbMetadataTable tableSchema = dataEntityTypeMap.DbTable;
            DbMetadataColumn pkColumn = tableSchema.PrimaryKey;
            SimplePropertyMap versionMap = dataEntityTypeMap.VersionProperty;
            SaveDataTable result = new SaveDataTable(tableSchema, dataEntities.Length);
            ISimpleProperty pk = dataEntityTypeMap.DataEntityType.PrimaryKey;
            ISaveMetaRow[] rows = result.SaveRows;

            for (int i = 0; i < dataEntities.Length; i++)
            {
                SaveRow tempVar = new SaveRow();
                if (pk != null)
                {
                    tempVar.Oid = new SyncColumnValuePair(pkColumn, pk, dataEntities[i]);
                }
                rows[i] = tempVar;
            }

            //if (tableSchema.IsLocale())
            //{
            //    IDataEntityProperty localeProp = dataEntityTypeMap.GetDataEntityType().GetProperty("localeid");
            //    for (int i2 = 0; i2 < dataEntities.Length; i2++)
            //    {
            //        rows[i2].SetLocale(new SimpleColumnValuePair(tableSchema.GetLocaleColumn(), localeProp.GetValueFast(dataEntities[i2])));
            //    }
            //}

            if (versionMap != null)
            {
                DbMetadataColumn versionColumn = versionMap.DbColumn;
                ISimpleProperty versionProperty = versionMap.DataEntityProperty;
                for (int i3 = 0; i3 < dataEntities.Length; i3++)
                {
                    rows[i3].Version = new SyncColumnValuePair(versionColumn, versionProperty, dataEntities[i3]);

                }
            }

            if (parentOids != null && parentOids.Length > 0)
            {
                if (parentOids.Length == 1)
                {
                    for (int i4 = 0; i4 < dataEntities.Length; i4++)
                    {

                        rows[i4].ParentOid = parentOids[0];
                    }
                    result.SetParentOid(parentOids[0]);
                }
                else
                {
                    for (int i5 = 0; i5 < dataEntities.Length; i5++)
                    {

                        rows[i5].ParentOid = parentOids[i5];

                    }
                }
            }

            dataSet.Tables.Add(result);
            return result;
        }

        private PkSnapshotSet EntitiesToSnapshot(object[] dataEntities, DbMetadataDatabase database, DataEntityTypeMap dataEntityTypeMap)
        {
            IDataEntityType dt = dataEntityTypeMap.DataEntityType;
            if (dataEntities.Length == 1)
            {
                return dt.GetPkSnapshot(dataEntities[0]);
            }

            int tableCount = database.Tables.Count;
            RuntimePkSnapshotSet tempVar = new RuntimePkSnapshotSet
            {
                PkSnapshotSet = new PkSnapshotSet(),
                Tables = new RuntimePkSnapshot[tableCount]
            };

            for (int i = 0; i < tableCount; i++)
            {
                PkSnapshot tempVar2 = new PkSnapshot
                {
                    TableName = database.Tables[i].Name
                };
                tempVar.PkSnapshotSet.Snapshots.Add(tempVar2);
                tempVar.Tables[i] = new RuntimePkSnapshot(tempVar2);
            }

            foreach (object dataEntity in dataEntities)
            {
                object pk = dt.PrimaryKey.GetValue(dataEntity);
                PkSnapshotSet snapshotSetTemp = dt.GetPkSnapshot(dataEntity);

                if (snapshotSetTemp != null)
                {
                    if (dt.IsQueryObj(dataEntity))
                    {
                        throw new Exception("传入的对象为查询对象，不支持保存操作!");
                    }

                    foreach (PkSnapshot item in snapshotSetTemp.Snapshots)
                    {
                        if (item.Oids != null)
                        {
                            int index = FindIndexOfDatabase(database, item.TableName);
                            foreach (object id in item.Oids)
                            {
                                tempVar.Tables[index].Oids.Add(id);
                            }

                            DbMetadataTable tableScheme = database.Tables.First(t => t.Name == item.TableName);
                            if (tableScheme.Seq != null)
                            {
                                foreach (object obj in item.Oids)
                                {
                                    tempVar.Tables[index].ParentIds.Add(pk);
                                }
                            }
                        }
                    }
                }
            }

            tempVar.Complete();
            return tempVar.PkSnapshotSet;
        }


        private int FindIndexOfDatabase(DbMetadataDatabase database, string tableName)
        {
            DbMetadataTableCollection tables = database.Tables;
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].Name.Equals(tableName))
                {
                    return i;
                }
            }
            throw new Exception($"ORM引擎保存实体时,从表结构[{database.Name}]中查找表[{tableName}]失败，表[{tableName}]不存在！");
        }

        /// <summary>
        /// 自动批处理执行。
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <param name="func">执行函数</param>
        /// <returns>执行结果数组</returns>
        private static object[] AutoBatchExecute(object[] ids, Func<object[], object[]> func)
        {
            //int batchSize = DataManagerUtils.GetBatchSize();
            int batchSize = 500;
            int realLen = ids.Length;

            // 去除尾部的null值
            for (int i = realLen - 1; i >= 0 && ids[i] == null; i--)
            {
                realLen--;
            }

            if (realLen == 0)
            {
                return EmptyObjectArray;
            }

            if (realLen <= batchSize)
            {
                if (realLen != ids.Length)
                {
                    object[] idsArray = ids;
                    ids = new object[realLen];
                    Array.Copy(idsArray, 0, ids, 0, realLen);
                }
                return func(ids);
            }

            List<object[]> ls = null;
            object[] batch = new object[batchSize];
            int mod = realLen % batchSize;
            int count = realLen / batchSize + (mod == 0 ? 0 : 1);

            for (int j = 0; j < count; j++)
            {
                if (j == count - 1 && mod != 0)
                {
                    batch = new object[mod];
                    Array.Copy(ids, j * batchSize, batch, 0, mod);
                }
                else
                {
                    Array.Copy(ids, j * batchSize, batch, 0, batchSize);
                }

                object[] values = func(batch);
                if (values != null)
                {
                    if (ls == null)
                    {
                        ls = new List<object[]>();
                    }
                    ls.Add(values);
                }
            }

            if (ls == null)
            {
                return null;
            }

            int c = 0;
            foreach (object[] values in ls)
            {
                c += values.Length;
            }

            object[] result = new object[c];
            int k = 0;
            foreach (object[] values in ls)
            {
                Array.Copy(values, 0, result, k, values.Length);
                k += values.Length;
            }

            return result;
     }
    

    public ISaveDataSet GetSaveDataSet(object[] dataEntities, bool includeDefaultValue)
        {
            throw new NotImplementedException();
        }

        public object Read(object oid)
        {
            if (oid==null)
            {
                throw new Exception("oid不能为空");
            }
            ReadWhere where = new ReadWhere(new Object[] { oid });
            object[] result = Read(where);
            if (result == null || result.Length == 0) return null;
            return result.FirstOrDefault();
            

        }

        public DbMetadataDatabase Database { get; private set; }

        public IDataEntityType DataEntityType
        {
            get
            {
                return this._dataEntityType;
            }
            set
            {
                Tuple<DataEntityTypeMap, DbMetadataDatabase> orAdd;
                Func<IDataEntityType, Tuple<DataEntityTypeMap, DbMetadataDatabase>> valueFactory = null;
                if (value == null)
                {
                    throw new Exception("设置数据管理器实体类型失败，实体类型DataEntityType不能为空！");
                }
                this._dataEntityType = value;
                
                bool cacheMetadata = this.Option.GetCacheMetadata();

                if (cacheMetadata)
                {
                    if (valueFactory == null)
                    {
                        valueFactory = delegate (IDataEntityType key) {
                            DbMetadataDatabase database = null;
                            return Tuple.Create<DataEntityTypeMap, DbMetadataDatabase>(DataEntityTypeMap.Build(value, ref database), database);
                        };
                    }
                    orAdd = _cache.GetOrAdd(value, valueFactory);
                }
                else
                {
                    DbMetadataDatabase database = null;
                    orAdd = Tuple.Create<DataEntityTypeMap, DbMetadataDatabase>(DataEntityTypeMap.Build(value, ref database), database);
                }
                this.DataEntityTypeMap = orAdd.Item1;
                this.Database = orAdd.Item2;
            }
        }
        public IDbDriver DbDriver { get; set; }

        public OperateOption Option
        {
            get
            {
                return this._option;
            }
            set
            {
                this._option = value;
            }
        }

       
    }
}
