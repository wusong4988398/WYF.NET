using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.DataEntity
{
    // 批量更新数据类
    public class BatchUpdateData : BatchInsertData
    {
        public List<DbMetadataColumn> Columns { get; set; }
        public List<object> OIds { get; set; } = new List<object>();
        public List<ISaveMetaRow> SaveRows { get; set; } = new List<ISaveMetaRow>();

        public BatchUpdateData(DbMetadataTable table, List<DbMetadataColumn> columns) : base(table)
        {
            Columns = columns;
        }

        public void Insert(IColumnValuePair[] inputValues, ISaveMetaRow saveData)
        {
            IColumnValuePair oid = saveData.Oid;
            base.Insert(inputValues, oid);
            OIds.Add(oid.Value);
            SaveRows.Add(saveData);
        }

        protected void SetDefaultValue(IColumnValuePair[] inputValues, int columnsCount, BitVector32 flag, object[] rowArray)
        {
            // 默认实现为空
        }

 
    }
}
