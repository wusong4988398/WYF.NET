using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.Drivers
{
    internal sealed class BulkCopyTaskContainer
    {
    
        private Dictionary<DbMetadataTable, BulkCopyData> _dataSet = new Dictionary<DbMetadataTable, BulkCopyData>();
        private KeyValuePair<DbMetadataTable, BulkCopyData> _last;

  
        public IDatabaseTask[] CreateTasks(OrmTransactionBase ormTran)
        {
            List<IDatabaseTask> list = new List<IDatabaseTask>();
            foreach (BulkCopyData data in this._dataSet.Values)
            {
                DbMetadataTable currentTable = data.Table;
                int tableLevel = OrmTransactionBase.GetTableLevel(currentTable);
                data.ConvertData(ormTran.GetTableConverter(currentTable, null));
                foreach (Tuple<string, DbMetadataColumn[]> tuple in currentTable.GetColumnsByTableGroup())
                {
                    string str = tuple.Item1;
                    DbMetadataColumn[] columns = tuple.Item2;
                    if ((ormTran.ExTableHaveRelitionField && (currentTable.ParentRelation != null)) && !string.IsNullOrEmpty(str))
                    {
                        DbMetadataColumn[] array = new DbMetadataColumn[columns.Length + 1];
                        columns.CopyTo(array, 0);
                        array[columns.Length] = currentTable.ParentRelation.ChildColumn;
                        columns = array;
                    }
                    BulkCopyDataReader dataReader = data.CreateDataReader(OrmTransactionBase.AutoTableName(currentTable.Name, str), columns);
                    int level = (tableLevel * 2) - (string.IsNullOrEmpty(str) ? 1 : 0);
                    list.Add(ormTran.CreateBulkCopyTask(dataReader.TableName, dataReader.Columns, dataReader, level));
                }
            }
            return list.ToArray();
        }

        private BulkCopyData GetBulkCopyData(DbMetadataTable table)
        {
            BulkCopyData data;
            if (this._last.Key == table)
            {
                return this._last.Value;
            }
            if (!this._dataSet.TryGetValue(table, out data))
            {
                data = new BulkCopyData(table);
                this._dataSet.Add(table, data);
            }
            this._last = new KeyValuePair<DbMetadataTable, BulkCopyData>(table, data);
            return data;
        }

        public void Insert(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair oid, OperateOption option = null)
        {
            this.GetBulkCopyData(table).Insert(inputValues, oid, option);
        }
    }
}
