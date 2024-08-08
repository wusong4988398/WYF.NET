using WYF.Bos.DataEntity.Metadata.Clr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.database
{
    public  class DataEntityTypeMap: DataEntityMetadataMapBase<IDataEntityType>
    {
        public DbMetadataTable DbTable { get; set; }

        public SimplePropertyMap PrimaryKey { get; set; }

        public SimplePropertyMap VersionProperty { get; set; }

        public List<SimplePropertyMap> SimpleProperties { get; set; }

        public List<ComplexPropertyMap> ComplexProperties { get; set; }

        public List<CollectionPropertyMap> CollectionProperties { get; set; }


        public IDataEntityType DataEntityType 
        { 
            get 
            {
                return this.MapSource;
            }

            set 
            { 
                base.MapSource = value; 
            }
        }

        private Object[] _properties;

        public static DataEntityTypeMap Build(IDataEntityType dataEntityType, ref DbMetadataDatabase db)
        {
            if (dataEntityType==null)
            {
                throw new ArgumentNullException("创建实体结构Map失败，参数实体类型[IDataEntityType]不能为空！");
            }
            db = new DbMetadataDatabase();
            DataEntityTypeMap tempVar = new DataEntityTypeMap();
            
            tempVar.DataEntityType = dataEntityType;
            tempVar.DbTable = new DbMetadataTable();
            DataEntityTypeMap dtMap = tempVar;
            db.Tables.Add(dtMap.DbTable);
            dtMap.DbTable.Name = AutoTableName("", dataEntityType.Name, dtMap.Alias);
            dtMap.DbTable.FullIndexName = dataEntityType.Name;

            BuildContext context = new BuildContext();
            context.ColumnPrefix = "";
            context.Database= db;
            BuildPrivate(dtMap, context);
            Validate(dtMap, context.Database, context.Errors);
            if (context.Errors.Count > 0)
                throw context.Errors[0];
            return dtMap;

        }

        private static void BuildPrivate(DataEntityTypeMap dtMap, BuildContext context)
        {
            
            dtMap._properties = new object[dtMap.DataEntityType.Properties.Count];
            BuildSimpleProperties(dtMap, context);
            BuildComplexProperties(dtMap, context);
            BuildCollectionProperties(dtMap, context);
        }

        public object GetPropertyMapByOrdinal(int ordinal)
        {
            return this._properties[ordinal];
        }
        private static void Validate(DataEntityTypeMap dtMap, DbMetadataDatabase database, List<Exception> errors)
        {
            
            Dictionary<string, DbMetadataTable> tables = new Dictionary<string, DbMetadataTable>(database.Tables.Count);
            if (database.Tables.Count==0)
            {
                errors.Add(new ORMDesignException("100001", $"实体{dtMap.Name}没有产生任何与数据库映射的表。"));
            }
            foreach (DbMetadataTable table in database.Tables)
            {
                if (string.IsNullOrEmpty(table.Name))
                {
                    errors.Add(new ORMDesignException("100001", $"发现名称为空的表，索引位置{database.Tables.IndexOf(table)}"));
                }
                if (!table.Name.StartsWith("#") && !IsValidIdentifier(table.Name))
                {
                    errors.Add(new ORMDesignException("100001", $"发现表名称{table.Name}不符合规范。"));
                }
                DbMetadataTable find = null;
                
                if ((find = tables[table.Name]) != null)
                {
                    errors.Add(new ORMDesignException("100001", $"发现重复的表名称{table.Name},索引位置:{database.Tables.IndexOf(find)}，{database.Tables.IndexOf(table)}"));
                }
                else
                {
                    tables[table.Name]= table;
                }
                Dictionary<string, DbMetadataColumn> columns = new Dictionary<string, DbMetadataColumn>(table.Columns.Count);
                if (columns.Count==0)
                {
                    errors.Add(new ORMDesignException("100001", $"表{table.Name}没有任何在数据库中映射的字段。"));

                }
                foreach (DbMetadataColumn column in table.Columns)
                {
                    if (string.IsNullOrEmpty(column.Name))
                    {
                        errors.Add(new ORMDesignException("100001", $"在表{table.Name}中发现名称为空的列，索引位置：{table.Columns.IndexOf(column)}。"));
                        continue;
                    }
                    if (!IsValidIdentifier(column.Name))
                    {
                        errors.Add(new ORMDesignException("100001", $"在表{table.Name}中发现字段名{column.Name}不符合规范。"));
                    }
                    DbMetadataColumn findColumn = columns[column.Name];
                    if (findColumn != null) { continue; }
                    columns[column.Name] = column;


                }



            }

        }
        private static bool IsValidIdentifier(string name)
        {
            return true;
        }
        private static void BuildCollectionProperties(DataEntityTypeMap dtMap, BuildContext context)
        {
            DbMetadataColumn parentColumn = null;
            DataEntityPropertyCollection dtProperties = dtMap.DataEntityType.Properties;
            List<CollectionPropertyMap> colList = new List<CollectionPropertyMap>(dtProperties.Count);
            foreach (ICollectionProperty col in dtProperties.GetCollectionProperties(true))
            {
                CollectionPropertyMap colMap = new CollectionPropertyMap();
                colMap.DataEntityProperty= col;
                DataEntityTypeMap newDtMap = new DataEntityTypeMap();
                newDtMap.DataEntityType = col.ItemType;
                DbMetadataTable tempVar3 = new DbMetadataTable();
                tempVar3.Name = AutoColumnName("", newDtMap.Name, newDtMap.Alias);
                newDtMap.DbTable = tempVar3;
                if (context.Database != null)
                    context.Database.Tables.Add(newDtMap.DbTable);

                BuildContext tempVar4 = new BuildContext();
                tempVar4.ColumnPrefix = "";
                tempVar4.Database = context.Database;
                BuildContext newContext = tempVar4;
                colMap.CollectionItemPropertyTypeMap= newDtMap;
                BuildPrivate(newDtMap, newContext);
                if (newDtMap.DbTable.PrimaryKey == null)
                {
                    throw new OrmException("DataEntityTypeMap", $"明细{newDtMap.DataEntityType.Name}没有定义主键");
                }
                DbMetadataColumn primaryKeyColumn = dtMap.DbTable.PrimaryKey;
                if (primaryKeyColumn==null)
                {
                    throw new OrmException("DataEntityTypeMap", $"明细{newDtMap.DataEntityType.Name}关联的主表{dtMap.DbTable.Name}没有主键");
                }
                if (newDtMap.DataEntityType is IEntryType) {
                    IDataEntityProperty seqProp = ((IEntryType)newDtMap.DataEntityType).SeqProperty;
                    if (seqProp != null)
                        newDtMap.DbTable.Seq = newDtMap.DbTable.Columns[seqProp.Alias];
                }
            
                bool tempVar5 = newDtMap.DbTable.Columns.TryGetValue(primaryKeyColumn.Name, out parentColumn);
                if (!tempVar5) {
                    parentColumn = primaryKeyColumn.Clone(primaryKeyColumn.Name);
                    newDtMap.DbTable.Columns.Add(parentColumn);
                    newDtMap.DbTable.SortColumns.Add(parentColumn.Name);
                    foreach (ISimpleProperty prop in col.ItemType.SortProperties)
                    {
                        newDtMap.DbTable.SortColumns.Add(prop.Alias);
                    }
                        
                }

                colMap.ParentColumn= parentColumn;
                DbMetadataRelation parentRelation = new DbMetadataRelation();
                parentRelation.ChildColumn= parentColumn;
                parentRelation.ParentTable = dtMap.DbTable;
                newDtMap.DbTable.Relations.Add(parentRelation);
                newDtMap.DbTable.ParentRelation = parentRelation;
                dtMap.DbTable.ChildTables.Add(newDtMap.DbTable);
                colList.Add(colMap);
                dtMap._properties[col.Ordinal] = colMap;
            }

            dtMap.CollectionProperties = colList;
        }

        private static void BuildComplexProperties(DataEntityTypeMap dtMap, BuildContext context)
        {
            DataEntityPropertyCollection dtProperties = dtMap.DataEntityType.Properties;
            List<ComplexPropertyMap> cpxList = new List<ComplexPropertyMap>(dtProperties.Count);
            foreach (IComplexProperty cpx in dtProperties.GetComplexProperties(true))
            {
                if (cpx.ComplexType==null) continue;
                ComplexPropertyMap cpxMap = new ComplexPropertyMap();
                cpxMap.DataEntityProperty = cpx;
                cpxMap.RefIdProperty = dtProperties[cpx.RefIdPropName];

                DataEntityTypeMap newDtMap = new DataEntityTypeMap();
                newDtMap.DataEntityType = cpx.ComplexType;
                DbMetadataTable cpxTable = new DbMetadataTable();
                cpxTable.Name = AutoColumnName("", newDtMap.Name, newDtMap.Alias);


                newDtMap.DbTable= cpxTable;
                cpxMap.ComplexPropertyTypeMap= newDtMap;
                BuildContext newContext = new BuildContext();
                newContext.ColumnPrefix= AutoColumnName(context.ColumnPrefix, cpx.Name, cpxMap.Alias);
                dtMap.DbTable.ChildTables.Add(newDtMap.DbTable);
                BuildPrivate(newDtMap, newContext);
                cpxList.Add(cpxMap);
                dtMap._properties[cpx.Ordinal] = cpxMap;
            }

            dtMap.ComplexProperties = cpxList;

        }

        private static void BuildSimpleProperties(DataEntityTypeMap dtMap, BuildContext context)
        {
            DbMetadataColumnCollection columns = dtMap.DbTable.Columns;
            DataEntityPropertyCollection dtProperties = dtMap.DataEntityType.Properties;
            List<SimplePropertyMap> spList = new List<SimplePropertyMap>(dtProperties.Count);
            ISimpleProperty primaryKey = dtMap.DataEntityType.PrimaryKey;
            ISet<string> fullIndexFields = dtMap.DataEntityType.FullIndexFields;

            foreach (ISimpleProperty sp in dtProperties.GetSimpleProperties(true))
            {
                SimplePropertyMap tempVar = new SimplePropertyMap();
                tempVar.DataEntityProperty = sp;
                SimplePropertyMap spMap = tempVar;
                DbMetadataColumn column = new DbMetadataColumn();
                column.Name = AutoColumnName(context.ColumnPrefix, sp.Name, spMap.Alias);
                column.ClrType = sp.PropertyType;
                column.IsEnableNull = sp.IsEnableNull;
                column.DbType = GetColumnDbType(context, sp, spMap);
                column.DbType = GetColumnDbType(context, sp, spMap);
                column.TableGroup= spMap.IsPrimaryKey ? null:spMap.TableGroup;
                column.AutoSync = spMap.AutoSync;
                //column.Encrypt = spMap.IsEncrypt;
                // column.IsNullable = spMap.IsEnableNull;

                column.IsNullable = GetIsNullable(sp.PropertyType);
                spMap.DbColumn = column;
                columns.Add(column);

                if (fullIndexFields.Contains(sp.Name))
                dtMap.DbTable.FullIndexProperties[column.Name] = sp;
                if (primaryKey != null && primaryKey.Name== sp.Name)
                {
                    if (dtMap.PrimaryKey == null)
                    {
                        dtMap.PrimaryKey = spMap;
                        dtMap.DbTable.PrimaryKey = spMap.DbColumn;
                    }
                    else
                    {
                        throw new OrmException("DataEntityTypeMap", $"复杂属性{sp.Name}指向的类型{sp.PropertyType.Name}上不允许标注主键。");
                    }
                }

                if (spMap.IsVersionProperty)
                {
                    if (dtMap.DbTable.VersionColumn != null)
                        context.Errors.Add(new OrmException("100001", $"实体{dtMap.Name}上定义了多个版本列。{dtMap.DbTable.VersionColumn.Name},{column.Name}"));
                    dtMap.VersionProperty = spMap;
                    dtMap.DbTable.VersionColumn = column;
                }
                spList.Add(spMap);
                dtMap._properties[sp.Ordinal] = spMap;


            }
            dtMap.SimpleProperties = spList;

        }
        private static bool GetIsNullable(Type type)
        {
            return ((type == typeof(string)) || (Nullable.GetUnderlyingType(type) != null));
        }
        private static string AutoTableName(string prefix, string name, string alias)
        {
            return AutoName(prefix, name, alias);
        }
        private static string AutoColumnName(string prefix, string name, string alias)
        {
            return AutoName(prefix, name, alias);
        }

        private static string AutoName(string prefix, string name, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                if (string.IsNullOrEmpty(prefix))
                    return name;

                return prefix + "_" + name;
            }
            return alias;
        }

        private static int GetColumnDbType(BuildContext context, ISimpleProperty sp, SimplePropertyMap spMap)
        {
            int dbType = spMap.DbType;
            return dbType;
        }
      


        private class BuildContext
        {
            public string ColumnPrefix { get; set; }

            public DbMetadataDatabase Database { get; set; }
            public List<Exception> Errors { get; set; }=new List<Exception>();

        }


    }
}
