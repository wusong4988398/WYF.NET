using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.Drivers
{
    // 表列转换器容器类
    public class TableColumnConverterContainer
    {
        private readonly Func<object, object>[] _converters;

        // 构造函数，初始化转换器数组
        public TableColumnConverterContainer(Func<object, object>[] converters)
        {
            _converters = converters;
        }

        // 尝试获取指定列的转换器
        public bool TryGetConverter(DbMetadataColumn column, out Func<object, object> converter)
        {
            converter = _converters[column.ColumnIndex];
            return converter != null;
        }

        // 获取列的数据库值
        public object GetColumnDbValue(IColumnValuePair pair)
        {
            Func<object, object> converter = _converters[pair.Column.ColumnIndex];
            if (converter != null)
            {
                return converter(pair.Value);
            }
            return pair.Value;
        }

        // 获取列的默认数据库值
        public object GetColumnDefaultDbValue(DbMetadataColumn column)
        {
            Func<object, object> converter = _converters[column.ColumnIndex];
            if (converter != null)
            {
                return converter(column.DefaultValue);
            }
            return column.DefaultValue;
        }
    }
}
