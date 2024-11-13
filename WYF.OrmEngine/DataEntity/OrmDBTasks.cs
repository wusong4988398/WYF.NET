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
    public class OrmDBTasks
    {
        private readonly List<IDatabaseTask> _lstSqlObj = new List<IDatabaseTask>();
        private readonly DBRoute _dbRoute;
        private BatchInsertTaskContainer _batchInsertTaskContainer;
        private BatchUpdateTaskContainer _batchUpdateTaskContainer;
        private readonly bool _isBatch;

        public OrmDBTasks(bool isBatch, DBRoute dbRoute)
        {
            _isBatch = isBatch;
            _dbRoute = dbRoute;
            if (isBatch)
            {
                _batchInsertTaskContainer = new BatchInsertTaskContainer(dbRoute);
                _batchUpdateTaskContainer = new BatchUpdateTaskContainer(dbRoute);
            }
        }

        public void Insert(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid)
        {
            if (_isBatch)
            {
                _batchInsertTaskContainer.Insert(table, inputValues, oid);
            }
            else
            {
                AddDbTasks(CRUDHelper.Insert(_dbRoute, table, inputValues, outputValues, oid));
            }
        }

        public void Update(ISaveDataTable table, ISaveMetaRow saveRow)
        {
            if (_isBatch)
            {
                _batchUpdateTaskContainer.Update(table, saveRow);
            }
            else
            {
                AddDbTasks(CRUDHelper.Update(_dbRoute, table.Schema, saveRow.DirtyValues.ToArray(), saveRow.Oid, saveRow.Version));
            }
        }

        public List<SqlTask> Delete(DbMetadataTable table, object[] oids, object[] originalVersions)
        {
            var tasks = CRUDHelper.Delete(_dbRoute, table, oids, originalVersions);
            AddDbTasks(tasks);
            return tasks;
        }

        public void CommitDbTask()
        {
            if (_batchInsertTaskContainer != null)
            {
                foreach (var dbTask in _batchInsertTaskContainer.CreateTasks())
                {
                    AddDbTask(dbTask);
                }
                _batchInsertTaskContainer = null;
            }

            if (_batchUpdateTaskContainer != null)
            {
                foreach (var dbTask in _batchUpdateTaskContainer.CreateTasks())
                {
                    AddDbTask(dbTask);
                }
                _batchUpdateTaskContainer = null;
            }

            ExecuteDbTasks(_lstSqlObj);
        }

        private void ExecuteDbTasks(List<IDatabaseTask> tasks)
        {
            throw new NotImplementedException();
            tasks.Sort((o1, o2) => o1.Level.CompareTo(o2.Level));
            foreach (var item in tasks)
            {
                //item.Execute();
            }
        }

        private void AddDbTasks(IEnumerable<SqlTask> dbTasks)
        {
            foreach (var task in dbTasks)
            {
                AddDbTask(task);
            }
        }

        private void AddDbTask(IDatabaseTask dbTask)
        {
            _lstSqlObj.Add(dbTask);
        }

        public BatchUpdateSeqTask UpdateSeq(DbMetadataTable table, List<Tuple<object, object, int>> list)
        {
            var dbTask = new BatchUpdateSeqTask(_dbRoute, table, list);
            AddDbTask(dbTask);
            return dbTask;
        }
    }
}
