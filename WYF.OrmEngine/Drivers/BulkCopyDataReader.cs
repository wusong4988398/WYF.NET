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
    internal sealed class BulkCopyDataReader : IDataReader, IDisposable, IDataRecord
    {
        // Fields
        private readonly DbMetadataColumn[] _columns;
        private int _currentIndex;
        private object[] _currentRow;
        private Dictionary<string, int> _nameCache;
        private object[][] _rows;
        private readonly string _tableName;

        // Methods
        public BulkCopyDataReader(string tableName, object[][] rows, DbMetadataColumn[] columns)
        {
            this._tableName = tableName;
            this._rows = rows;
            this._columns = columns;
            this._currentIndex = -1;
        }

        public void Close()
        {
            this._currentIndex = -1;
            this._rows = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        public byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotSupportedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotSupportedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotSupportedException();
        }

        public double GetDouble(int i)
        {
            throw new NotSupportedException();
        }

        public Type GetFieldType(int i)
        {
            if ((i < 0) || (i >= this._columns.Length))
            {
                throw new ArgumentOutOfRangeException("i");
            }
            return this._columns[i].ClrType;
        }

        public float GetFloat(int i)
        {
            throw new NotSupportedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i)
        {
            throw new NotSupportedException();
        }

        public int GetInt32(int i)
        {
            throw new NotSupportedException();
        }

        public long GetInt64(int i)
        {
            throw new NotSupportedException();
        }

        public string GetName(int i)
        {
            if ((i < 0) || (i >= this._columns.Length))
            {
                throw new ArgumentOutOfRangeException("i");
            }
            return this._columns[i].Name;
        }

        public int GetOrdinal(string name)
        {
            int num2;
            if (this._nameCache == null)
            {
                this._nameCache = new Dictionary<string, int>(this._columns.Length, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < this._columns.Length; i++)
                {
                    this._nameCache.Add(this._columns[i].Name, i);
                }
            }
            if (this._nameCache.TryGetValue(name, out num2))
            {
                return num2;
            }
            return -1;
        }

        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public string GetString(int i)
        {
            throw new NotSupportedException();
        }

        public object GetValue(int i)
        {
            DbMetadataColumn column = this._columns[i];
            return this._currentRow[column.ColumnIndex];
        }

        public int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotSupportedException();
        }

        public bool NextResult()
        {
            throw new NotSupportedException();
        }

        public bool Read()
        {
            this._currentIndex++;
            if (this._currentIndex < this._rows.Length)
            {
                this._currentRow = this._rows[this._currentIndex];
                return true;
            }
            this._currentRow = null;
            return false;
        }

        // Properties
        public ReadOnlyCollection<DbMetadataColumn> Columns
        {
            get
            {
                return new ReadOnlyCollection<DbMetadataColumn>(this._columns);
            }
        }

        public int Depth
        {
            get
            {
                return 0;
            }
        }

        public int FieldCount
        {
            get
            {
                return this._columns.Length;
            }
        }

        public bool IsClosed
        {
            get
            {
                return (this._currentIndex < 0);
            }
        }

        public object this[string name]
        {
            get
            {
                return this.GetValue(this.GetOrdinal(name));
            }
        }

        public object this[int i]
        {
            get
            {
                return this.GetValue(i);
            }
        }

        public int RecordsAffected
        {
            get
            {
                return this._rows.Length;
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
