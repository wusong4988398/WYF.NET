using WYF.Bos.Orm.Metadata.DataEntity;


namespace WYF.Bos.DataEntity.Metadata.database
{
    public class DbMetadataTable : DbMetadataBase
    {

        public DataSourceSchemaType SchemaType { get; set; }
        public string FullIndexName { get; set; }


        public DbMetadataColumn Seq { get; set; }

        public Dictionary<string, ISimpleProperty> FullIndexProperties { get; set; }=new Dictionary<string, ISimpleProperty>();


        private Tuple<string, DbMetadataColumn[]>[] _columnsByTableGroupCache;

        public DbMetadataTableCollection ChildTables { get; private set; }

        public DbMetadataColumnCollection Columns { get; private set; }

        public List<string> SortColumns { get; set; }

        public DbMetadataRelation ParentRelation { get; set; }

        public DbMetadataColumn PrimaryKey { get; set; }

        public DbMetadataRelationCollection Relations { get; private set; }


        public DataEntityTypeMap DataEntityTypeMap { get; set; }

        public DbMetadataColumn VersionColumn { get; set; }
        public DbMetadataTable()
        {

            this.Columns = new DbMetadataColumnCollection(this);
            this.Relations = new DbMetadataRelationCollection();
            this.ChildTables = new DbMetadataTableCollection();
        }
        public Tuple<string, DbMetadataColumn[]>[] GetColumnsByTableGroup()
        {
            return this.GetColumnsByTableGroup(this.Columns, true, true);
        }

        public Tuple<string, DbMetadataColumn[]>[] GetColumnsByTableGroup(IList<DbMetadataColumn> columns, bool useCache, bool includeExtTablePK)
        {
            Tuple<string, DbMetadataColumn[]>[] tupleArray = null;
            if ((this._columnsByTableGroupCache == null) || !useCache)
            {
                List<DbMetadataColumn> list = new List<DbMetadataColumn>(columns.Count);
                Dictionary<string, List<DbMetadataColumn>> dictionary = null;
                foreach (DbMetadataColumn column in columns)
                {
                    if (string.IsNullOrEmpty(column.TableGroup))
                    {
                        list.Add(column);
                    }
                    else
                    {
                        List<DbMetadataColumn> list2;
                        if (dictionary == null)
                        {
                            dictionary = new Dictionary<string, List<DbMetadataColumn>>(2);
                        }
                        if (!dictionary.TryGetValue(column.TableGroup, out list2))
                        {
                            list2 = new List<DbMetadataColumn>();
                            if (includeExtTablePK)
                            {
                                list2.Add(this.PrimaryKey);
                            }
                            dictionary.Add(column.TableGroup, list2);
                        }
                        list2.Add(column);
                    }
                }
                int num = ((dictionary == null) ? 0 : dictionary.Count) + 1;
                Tuple<string, DbMetadataColumn[]>[] tupleArray2 = new Tuple<string, DbMetadataColumn[]>[num];
                tupleArray2[0] = new Tuple<string, DbMetadataColumn[]>(string.Empty, list.ToArray());
                if (dictionary != null)
                {
                    int index = 1;
                    foreach (KeyValuePair<string, List<DbMetadataColumn>> pair in dictionary)
                    {
                        tupleArray2[index] = new Tuple<string, DbMetadataColumn[]>(pair.Key, pair.Value.ToArray());
                        index++;
                    }
                }
                if (useCache)
                {
                    this._columnsByTableGroupCache = tupleArray2;
                }
                else
                {
                    tupleArray = tupleArray2;
                }
            }
            if (useCache)
            {
                return this._columnsByTableGroupCache;
            }
            return tupleArray;
        }

        public bool IsSubEntry
        {
            get
            {
                if (this.ParentRelation != null &&
          this.ParentRelation.ParentTable.ParentRelation != null &&
          this.DataEntityTypeMap.DataEntityType is IEntryType)
                    return true;
                return false;
            }
        
        }


    }
}
