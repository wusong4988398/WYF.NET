using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine.db;
using WYF.OrmEngine.Drivers;

namespace WYF.OrmEngine.DataEntity
{
    // 批量更新任务容器类
    public class BatchUpdateTaskContainer
    {
        private Dictionary<string, BatchUpdateData> _dataSet = new Dictionary<string, BatchUpdateData>();
        private DBRoute DbRoute;

        public BatchUpdateTaskContainer(DBRoute dbRoute)
        {
            DbRoute = dbRoute;
        }

        public void Update(ISaveDataTable table, ISaveMetaRow saveRow)
        {
            IColumnValuePair[] inputValues = saveRow.DirtyValues.ToArray();
            GetBatchUpdateData(GetBatchTaskKey(table.Schema, inputValues)).Insert(inputValues, saveRow);
        }

        private Tuple<string, List<DbMetadataColumn>, DbMetadataTable> GetBatchTaskKey(DbMetadataTable table, IColumnValuePair[] inputValues)
        {
            StringBuilder key = new StringBuilder();
            List<DbMetadataColumn> columns = new List<DbMetadataColumn>(inputValues.Length);
            foreach (IColumnValuePair colVal in inputValues)
            {
                key.Append(colVal.Column.Name).Append('.');
                columns.Add(colVal.Column);
            }
            key.Append(table.Name);
            return new Tuple<string, List<DbMetadataColumn>, DbMetadataTable>(key.ToString(), columns, table);
        }

        private BatchUpdateData GetBatchUpdateData(Tuple<string, List<DbMetadataColumn>, DbMetadataTable> key)
        {
            BatchUpdateData task;
            if (!_dataSet.TryGetValue(key.Item1, out task))
            {
                task = new BatchUpdateData(key.Item3, key.Item2);
                _dataSet[key.Item1] = task;
            }
            return task;
        }

        public List<IDatabaseTask> CreateTasks()
        {
            List<IDatabaseTask> result = new List<IDatabaseTask>();
            foreach (BatchUpdateData data in _dataSet.Values)
            {
                DbMetadataTable table = data.Table;
                int tableLevel = CRUDHelper.GetTableLevel(table);
                List<Tuple<string, DbMetadataColumn[]>> columnsByTableGroup = table.GetColumnsByTableGroup(data.Columns, false, false).ToList();
                foreach (Tuple<string, DbMetadataColumn[]> group in columnsByTableGroup)
                {
                    string groupName = group.Item1;
                    DbMetadataColumn[] columns = group.Item2;
                    if (columns.Length > 0)
                    {
                        int level = tableLevel * 2 - (groupName.IsNullOrWhiteSpace()? 1 : 0);
                        BatchUpdateTask task = new BatchUpdateTask(DbRoute, CRUDHelper.GetTableNameWithGroup(table.Name, groupName), columns, data.Rows, table.PrimaryKey, data.OIds, data.SaveRows, level);
                        result.Add(task);
                    }
                }
            }
            return result;
        }
    }
}
