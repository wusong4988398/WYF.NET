using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.DataEntity
{
    public sealed class SyncColumnValuePair : IColumnValuePair, ICloneable
    {
        private IDataEntityProperty _sp;
        private object _dataEntity;
        private DbMetadataColumn _column;

        public DbMetadataColumn Column => this._column;

        public object Value {
            get { return this._sp.GetValue(this._dataEntity); }
            set { this._sp.SetValue(this._dataEntity, value); }
        }

        public SyncColumnValuePair() { }

        public SyncColumnValuePair(DbMetadataColumn column, IDataEntityProperty sp, object dataEntity)
        {
            if (sp == null) throw new ArgumentNullException(nameof(sp));
            if (dataEntity == null) throw new ArgumentNullException(nameof(dataEntity));

            this._column = column;
            this._sp = sp;
            this._dataEntity = dataEntity;
        }

    


        public object Clone()
        {
            SyncColumnValuePair varCopy = new SyncColumnValuePair();
            varCopy._sp = this._sp;
            varCopy._dataEntity = this._dataEntity;
            varCopy._column = this._column;
            return varCopy;
        }
    }
}