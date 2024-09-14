using Antlr4.Runtime.Misc;
using WYF.Bos.Collections.Generic;
using WYF.Bos.DataEntity;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.db;
using WYF.Bos.Orm.datamanager;
using WYF.Bos.Orm.DataManager;
using WYF.Bos.Orm.Drivers;
using WYF.Bos.Orm.Exceptions;
using WYF.Bos.Orm.Metadata.DataEntity;
using WYF.Bos.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;



namespace WYF.Bos.Orm.dataentity
{
    public class DataManagerImplement : IDataManager
    {
        private int batchDeleteSize = 10000;

        private int maxObjects = 1000000;
        public bool SelectHeadOnly { get; set; } = false;
        private TableAliasGenner aliasGenner = new TableAliasGenner();
        private static readonly Object[] EmptyDynmaicObjects = new DynamicObject[0];
        private static readonly Object[] EmptyObjects = new Object[0];
        public DataEntityTypeMap DataEntityTypeMap { get; set; }
        private Dictionary<DbMetadataTable, EntryInfo> entryPageInfo = null;
        public DbMetadataDatabase DataDatabase { get; set; }
        private int? pageSize;
        private int startRowIndex;
        private DBRoute _dbRoute = null;
        private IDataEntityType _dataEntityType;
        private ObjectCache<IDataEntityType, Tuple<DataEntityTypeMap, DbMetadataDatabase>> _cache;


        public DataManagerImplement(IDataEntityType dt):this(dt, null)
        {
           
        }

        public DataManagerImplement(IDataEntityType dt, DBRoute dbRoute)
        {
            //this._cache = ObjectCache.Create();
            this._dbRoute = dbRoute;
            SetDataEntityType(dt);
            Init();
        }
        public DataManagerImplement()
        {
            //this._cache = ObjectCache.create();
            Init();
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
           
            QuickDataSet dataSet = ReadToDataSet(this.DataDatabase, this.DataEntityTypeMap.DbTable, where);
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
            dataSet.Tables.TryGetValue(rootTableName,out QuickDataTable rootTable);
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
                    throw new ORMDesignException("100001",
                        String.Format("表{0}中读取出的数据，出现重复的主键({1})数据:{2}",rootTableName, rootTable.Schema.PrimaryKey.Name, pkOid), e);
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
                RuntimePkSnapshotSet snapshotSet = pkDict.GetValueOrDefault(parentOid,null);
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
                            throw new ORMDesignException("100001",
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
                if (dt is DynamicObjectType) {
                    DynamicObject[] arrayOfDynamicObject = new DynamicObject[rows.Length];
                } else
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
                        if (dataEntity is ISupportInitialize){
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

        public  QuickDataSet Select(Collection<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, ReadWhere where)
        {
            Collection<DbMetadataTable> tables = (Collection<DbMetadataTable>)tablesSchema;
            List<SelectSql> selectSQLAll = new List<SelectSql>(tables.Count);
            SqlBuilder whereSql = GetRootTableWhereSQl(rootTable, where);
            foreach (DbMetadataTable tableSchema in tablesSchema)
            {
                DbMetadataTable stopJoinTable = null;
                if (tableSchema!= rootTable)
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
                if (!whereSql.IsEmpty())
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
                if (sqlCount.Length > 0 && !whereSql.IsEmpty())
                {
                    sqlCount = String.Format(sqlCount, "WHERE");
                    countBuild.Append("/*ORM*/ ").Append(sqlCount).AppendSqlBuilder(whereSql)
                      .Append((tupSQL.CountWhere.Length > 0) ? (" AND " + tupSQL.CountWhere) : " ", (tupSQL.CountParams != null) ? tupSQL.CountParams
                        .ToArray() : null)
                      .Append(tupSQL.CountGroupBySqlpart.IsNotEmptyOrNull() ? " " : tupSQL.CountGroupBySqlpart, new Object[0]);
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
                if (this.SelectHeadOnly && tableSchema != rootTable && (
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
                          table.Rows.Length >= entryInfo.PageSize && !selectInfo.CountSqlBuild.IsEmpty())
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
                    row.Values=new object[columnCount];
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


        private T DoExecute<T>(SqlBuilder builder, ReadWhere where, DbMetadataTable tableSchema, DbMetadataTable rootTable,  Func<IDataReader,T> action)
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
            List<SqlParameter> parameters = new List<SqlParameter>();
            List<SqlParameter> Joinparameters = new List<SqlParameter>();
            DbMetadataRelation parentRelation = currentTable.ParentRelation;
            DbMetadataTable childTable = currentTable;
            String parentField = null;
            if (this.entryPageInfo != null)
            {
                DbMetadataColumn seq = currentTable.Seq;
                if (seq != null)
                {
                    EntryInfo pageInfo = this.entryPageInfo.GetValueOrDefault(currentTable);
                    if (pageInfo.StartRowIndex== 0)
                    {
                        parentField = GetColumnNameSql(childTable.Name, parentRelation.ChildColumn.Name);
                        sqlCount.Append(" SELECT ").Append(parentField).Append(", Count(*)   FROM ");
                    }
                    sqlWhere.Append(GetColumnNameSql(currentTable.Name, seq.Name)).Append(" BETWEEN ? AND ? ");
                    parameters.Add(new SqlParameter(4, pageInfo.StartRowIndex + 1));
                    parameters.Add(new SqlParameter(4, pageInfo.StartRowIndex + pageInfo.PageSize));
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
                        Joinparameters.Add(new SqlParameter(4, pageInfo.StartRowIndex + 1));
                        Joinparameters
                          .Add(new SqlParameter(4, pageInfo.StartRowIndex + pageInfo.PageSize));
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
                
                if (column.TableGroup.IsNotEmptyOrNull() &&
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
            if (!StringUtils.IsEmpty(where.WhereSql))
                return rootwhereSql;
            SqlBuilder whereSql = new SqlBuilder();
            DbMetadataTable table = childTable;
            while (rootTable != table.ParentRelation.ParentTable)
                table = table.ParentRelation.ParentTable;
            String pk = GetColumnNameSql(table.Name, table.ParentRelation.ChildColumn.Name);
            if (where.IsSingleValue)
            {
                SqlParameter pkValueParameter = new SqlParameter(":PKValue", rootTable.PrimaryKey.DbType, where.ReadOids[0]);
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
                SqlParameter pkValueParameter = new SqlParameter(":PKValue", rootTable.PrimaryKey.DbType, where.ReadOids[0]);
                build.Append(pk + "= ?", new Object[] { pkValueParameter });
            }
            else if (where.ReadOids != null)
            {
                build.AppendSqlBuilder(GetIdsWhereSql(rootTable.PrimaryKey.DbType, pk, where));
            }
            if (!StringUtils.IsEmpty(where.WhereSql))
            {
                if (!build.IsEmpty())
                    build.Append(" AND ", new Object[0]);
                build.Append(where.WhereSql, (where.SqlParams != null) ? where.SqlParams.ToArray() : null);
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
            if (StringUtils.IsEmpty(column.TableGroup))
                return GetColumnNameSql(currentTable.Name, column.Name);
            return GetColumnNameSql(currentTable.Name + '_' + column.TableGroup, column.Name);
        }

        private void CreateEntryPageInfo(int startRowIndex, int pageSize, DbMetadataTable tableSchema)
        {
            if (this.entryPageInfo == null)
                this.entryPageInfo = new Dictionary<DbMetadataTable, EntryInfo>();
            EntryInfo pageInfo = new EntryInfo();
            pageInfo.PageSize= pageSize;
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
            bool cacheMetadata = DataManagerUtils.GetCacheMetadata();
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
            this.DataDatabase=temp.Item2;
   
        }
        private void CheckDBRoute(IDataEntityType dt)
        {
            if (this._dbRoute.RouteKey == null || this._dbRoute.RouteKey.Trim().Length == 0)
                throw new ArgumentException($"ORM: 元数据{dt.Name}的routeKey为空!");
        }
    }
}
