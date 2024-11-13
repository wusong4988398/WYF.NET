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
    public class BatchInsertTaskContainer
    {
        private Dictionary<DbMetadataTable, BatchInsertData> _dataSet = new Dictionary<DbMetadataTable, BatchInsertData>();

        private Tuple<DbMetadataTable, BatchInsertData> _last;

        private DBRoute dbRoute;

        public BatchInsertTaskContainer(DBRoute dbRoute)
        {
            this.dbRoute = dbRoute;
        }

        public void Insert(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair oid)
        {
            GetBatchInsertData(table).Insert(inputValues, oid);
        }

        private BatchInsertData GetBatchInsertData(DbMetadataTable table)
        {
            if (_last != null && _last.Item1 == table)
                return _last.Item2;

            if (!_dataSet.TryGetValue(table, out var task))
            {
                task = new BatchInsertData(table);
                _dataSet[table] = task;
            }

            _last = Tuple.Create(table, task);
            return task;
        }

        public IDatabaseTask[] CreateTasks()
        {
            List<IDatabaseTask> result = new List<IDatabaseTask>();
            foreach (var data in _dataSet.Values)
            {
                DbMetadataTable table = data.Table;
                int tableLevel = CRUDHelper.GetTableLevel(table);
                List<Tuple<string, DbMetadataColumn[]>> columnsByTableGroup = table.GetColumnsByTableGroup().ToList();

                foreach (var group in columnsByTableGroup)
                {
                    string groupName = group.Item1;
                    DbMetadataColumn[] columns = group.Item2;

                    if (CRUDHelper.GetExTableHaveRelitionField() && table.ParentRelation != null &&
                        !string.IsNullOrWhiteSpace(groupName))
                    {
                        var columns2 = new DbMetadataColumn[columns.Length + 1];
                        Array.Copy(columns, 0, columns2, 0, columns.Length);
                        columns2[columns.Length] = table.ParentRelation.ChildColumn;
                        columns = columns2;
                    }

                    int level = tableLevel * 2 - (string.IsNullOrWhiteSpace(groupName) ? 1 : 0);
                    var task = new BatchInsertTask(dbRoute, CRUDHelper.GetTableNameWithGroup(table.Name, groupName), columns, level, data.Rows);
                    result.Add(task);
                }
            }

            return result.ToArray();
        }
    }
}
