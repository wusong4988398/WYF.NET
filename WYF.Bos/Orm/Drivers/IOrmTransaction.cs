using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;
using JNPF.Form.DataEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Drivers
{
    public interface IOrmTransaction : IDbTransaction, IDisposable
    {
        event EventHandler CommitAfter;
        void CommitSqlTask();
        void Delete(DbMetadataTable table, object[] oids, object[] originalVersions, OperateOption option = null);
        void Delete(IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, object[] rootOids, OperateOption option = null);
        void Insert(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, OperateOption option = null);
        void Update(DbMetadataTable table, IColumnValuePair[] inputValues, IColumnValuePair[] outputValues, IColumnValuePair oid, IColumnValuePair originalVersion, OperateOption option = null);
    }
}
