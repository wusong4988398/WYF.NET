using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.DataManager
{
    internal struct SimpleColumnValuePair : IColumnValuePair
    {
        private readonly DbMetadataColumn _column;
        private readonly object _value;
        public SimpleColumnValuePair(DbMetadataColumn column, object value)
        {
            this._column = column;
            if (this._column.ClrType.Equals(typeof(string)) && (value == null))
            {
                this._value = " ";
            }
            else if (this._column.ClrType.Equals(typeof(string)) && string.IsNullOrWhiteSpace(value.ToString()))
            {
                this._value = " ";
            }
            else
            {
                this._value = value;
            }
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
                return this._value;
            }
            set
            {
                throw new ReadOnlyException();
            }
        }
    }
}
