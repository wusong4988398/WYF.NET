using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Drivers
{
    internal sealed class TableColumnConverterContainer
    {

        private Func<object, object>[] _converters;
        public TableColumnConverterContainer(Func<object, object>[] converters)
        {
            this._converters = converters;
        }

        public object GetColumnDbValue(IColumnValuePair pair)
        {
            Func<object, object> func = this._converters[pair.Column.ColumnIndex];
            if (func != null)
            {
                return func(pair.Value);
            }
            return pair.Value;
        }

        public bool TryGetConverter(DbMetadataColumn column, out Func<object, object> converter)
        {
            converter = this._converters[column.ColumnIndex];
            return (converter != null);
        }
    }

}
