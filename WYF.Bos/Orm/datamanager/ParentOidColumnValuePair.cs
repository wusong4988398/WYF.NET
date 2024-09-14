using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal struct ParentOidColumnValuePair : IColumnValuePair
    {
        private readonly IDataEntityProperty _sp;
        private readonly object _dataEntity;
        private readonly IColumnValuePair _parentProxy;
        private readonly DbMetadataColumn _column;
        public ParentOidColumnValuePair(DbMetadataColumn column, IDataEntityProperty sp, object dataEntity, IColumnValuePair parentProxy)
        {
            this._column = column;
            this._sp = sp;
            this._dataEntity = dataEntity;
            this._parentProxy = parentProxy;
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
                return this._parentProxy.Value;
            }
            set
            {
                if ((this._sp != null) && (this._dataEntity != null))
                {
                    this._sp.SetValue(this._dataEntity, value);
                }
            }
        }
    }
}
