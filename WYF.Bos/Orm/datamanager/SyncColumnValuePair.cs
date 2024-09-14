using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal struct SyncColumnValuePair : IColumnValuePair
    {
        private readonly IDataEntityProperty _sp;
        private readonly object _dataEntity;
        private readonly DbMetadataColumn _column;
        public SyncColumnValuePair(DbMetadataColumn column, IDataEntityProperty sp, object dataEntity)
        {
            this._column = column;
            this._sp = sp;
            this._dataEntity = dataEntity;
        }

        public DbMetadataColumn Column
        {
            get
            {
                return this._column;
            }
        }
        public object Value
        {
            get
            {
                return this._sp.GetValue(this._dataEntity);
            }
            set
            {
                this._sp.SetValue(this._dataEntity, value);
            }
        }
    }
}
