using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.Drivers
{
    internal class BulkCopyData
    {
        // Fields
        private ForWriteList<object[]> _rows;
        private object[][] _rowsByArray;
        private DbMetadataTable _table;

        // Methods
        internal BulkCopyData(DbMetadataTable table)
        {
            this._table = table;
            this._rows = new ForWriteList<object[]>();
        }

        internal virtual void ConvertData(TableColumnConverterContainer converter)
        {
            this._rowsByArray = this._rows.ToArray();
            this._rows = null;
            foreach (DbMetadataColumn column in this.GetConvertColumns())
            {
                Func<object, object> func;
                if (converter.TryGetConverter(column, out func))
                {
                    int columnIndex = column.ColumnIndex;
                    for (int i = 0; i < this._rowsByArray.Length; i++)
                    {
                        object[] objArray = this._rowsByArray[i];
                        objArray[columnIndex] = func(objArray[columnIndex]);
                    }
                }
            }
        }

        internal BulkCopyDataReader CreateDataReader(string tableName, DbMetadataColumn[] columns)
        {
            return new BulkCopyDataReader(tableName, this._rowsByArray, columns);
        }

        private object[] CreateRowArray()
        {
            object[] item = new object[this._table.Columns.Count];
            this._rows.Add(item);
            return item;
        }

        protected virtual IList<DbMetadataColumn> GetConvertColumns()
        {
            return this._table.Columns;
        }

        public virtual void Insert(IColumnValuePair[] inputValues, IColumnValuePair oid, OperateOption option = null)
        {
            int count = this._table.Columns.Count;
            BitArray flag = new BitArray(count);
            object[] rowArray = this.CreateRowArray();
            foreach (IColumnValuePair pair in inputValues)
            {
                int columnIndex = pair.Column.ColumnIndex;
                rowArray[columnIndex] = pair.Value;
                flag[columnIndex] = true;
            }
            this.SetDefaultValue(inputValues, count, flag, rowArray);
        }

        protected virtual void SetDefaultValue(IColumnValuePair[] inputValues, int columnsCount, BitArray flag, object[] rowArray)
        {
            if (inputValues.Length < columnsCount)
            {
                foreach (DbMetadataColumn column in this._table.Columns)
                {
                    int columnIndex = column.ColumnIndex;
                    if (!flag[columnIndex])
                    {
                        rowArray[columnIndex] = column.DefaultValue;
                    }
                }
            }
        }

     
        public DbMetadataTable Table
        {
            get
            {
                return this._table;
            }
        }
    }
}
