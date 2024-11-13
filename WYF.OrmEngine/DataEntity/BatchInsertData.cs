using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.OrmEngine.Drivers;

namespace WYF.OrmEngine.DataEntity
{
    // 批量插入数据的容器类
    public class BatchInsertData
    {
        private DbMetadataTable _table;
        private List<object[]> _rows = new List<object[]>();
        private List<IColumnValuePair> _oids = new List<IColumnValuePair>();
        private TableColumnConverterContainer _columnConverter;

        // 获取所有行数据
        public List<object[]> Rows => _rows;

        // 获取所有OID（唯一标识符）
        public List<IColumnValuePair> OIds => _oids;

        // 构造函数，初始化表信息和列转换器
        public BatchInsertData(DbMetadataTable table)
        {
            _table = table;
            _columnConverter = CRUDHelper.GetTableConverter(table);
        }

        // 插入一行数据
        public void Insert(IColumnValuePair[] inputValues, IColumnValuePair oid)
        {
            int columnsCount = _table.Columns.Count;
            BitSet flag = new BitSet(columnsCount);
            object[] rowArray = CreateRowArray();
            _oids.Add(oid);

            // 遍历输入值，设置到行数组中
            foreach (IColumnValuePair columnValue in inputValues)
            {
                int index = columnValue.Column.ColumnIndex;
                rowArray[index] = _columnConverter.GetColumnDbValue(columnValue);
                flag.Set(index, true);
            }

            // 设置默认值
            SetDefaultValue(inputValues, columnsCount, flag, rowArray);
        }

        // 设置未提供值的列的默认值
        protected void SetDefaultValue(IColumnValuePair[] inputValues, int columnsCount, BitSet flag, object[] rowArray)
        {
            if (inputValues.Length < columnsCount)
            {
                foreach (DbMetadataColumn column in _table.Columns)
                {
                    int index = column.ColumnIndex;
                    if (!flag.Get(index))
                    {
                        rowArray[index] = _columnConverter.GetColumnDefaultDbValue(column);
                    }
                }
            }
        }

        // 创建一个新的行数组并添加到行列表中
        private object[] CreateRowArray()
        {
            object[] rowArray = new object[_table.Columns.Count];
            _rows.Add(rowArray);
            return rowArray;
        }

        // 获取表信息
        public DbMetadataTable Table => _table;
    }

    // BitSet 类的简单实现
    public class BitSet
    {
        private bool[] bits;

        public BitSet(int size)
        {
            bits = new bool[size];
        }

        public void Set(int index, bool value)
        {
            if (index >= 0 && index < bits.Length)
            {
                bits[index] = value;
            }
        }

        public bool Get(int index)
        {
            if (index >= 0 && index < bits.Length)
            {
                return bits[index];
            }
            return false;
        }
    }
}