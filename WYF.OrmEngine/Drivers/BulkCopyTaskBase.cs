using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.Drivers
{
    public abstract class BulkCopyTaskBase : IDatabaseTask, IDisposable
    {

        private readonly ReadOnlyCollection<DbMetadataColumn> _columns;
        private readonly IDataReader _dataReader;
        private readonly int _level;
        private readonly string _tableName;


        protected BulkCopyTaskBase(string tableName, ReadOnlyCollection<DbMetadataColumn> columns, IDataReader dataReader, int level)
        {
            this._tableName = tableName;
            this._columns = columns;
            this._dataReader = dataReader;
            this._level = level;
        }

        public virtual void Dispose()
        {
            if (this._dataReader != null)
            {
                this._dataReader.Close();
                this._dataReader.Dispose();
            }
        }

        public abstract void Execute(IDbConnection con, IDbTransaction tran);


        public ReadOnlyCollection<DbMetadataColumn> Columns
        {
            get
            {
                return this._columns;
            }
        }

        public IDataReader DataReader
        {
            get
            {
                return this._dataReader;
            }
        }

        public int Level
        {
            get
            {
                return this._level;
            }
        }

        public string TableName
        {
            get
            {
                return this._tableName;
            }
        }
    }
}
