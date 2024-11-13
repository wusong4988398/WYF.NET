using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.dataManager
{
    public class ParentOidColumnValuePair : IColumnValuePair, ICloneable
    {
        private IDataEntityProperty _sp;
        private object _dataEntity;
        private IColumnValuePair _parentProxy;
        private DbMetadataColumn _column;

        public DbMetadataColumn Column => this._column;

        public object Value {
            get { return this._parentProxy?.Value; }
            set {

                if (this._sp != null && this._dataEntity != null)
                {
                    this._sp.SetValue(this._dataEntity, value);
                }
            }
        }

        public ParentOidColumnValuePair() { }

        public ParentOidColumnValuePair(DbMetadataColumn column, IDataEntityProperty sp, object dataEntity, IColumnValuePair parentProxy)
        {
            this._column = column;
            this._sp = sp;
            this._dataEntity = dataEntity;
            this._parentProxy = parentProxy;
        }
        public object Clone()
        {
            ParentOidColumnValuePair varCopy = new ParentOidColumnValuePair();
            varCopy._sp = this._sp;
            varCopy._dataEntity = this._dataEntity;
            varCopy._parentProxy = this._parentProxy;
            varCopy._column = this._column;
            return varCopy;
        }
    }
}