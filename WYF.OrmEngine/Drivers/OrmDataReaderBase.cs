using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.Drivers
{
    public abstract class OrmDataReaderBase : IDataReader, IDisposable, IDataRecord
    {
    
        private ConverterPair[] _converters;
        private IDataReader _parent;
        private DbMetadataTable _tableSchema;

 
        protected OrmDataReaderBase()
        {
        }

        public void ChangeParent(IDataReader reader, DbMetadataTable tableSchema)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (tableSchema == null)
            {
                throw new ArgumentNullException("tableSchema");
            }
            this._parent = reader;
            this._tableSchema = tableSchema;
            this._converters = this.GetConverters(tableSchema);
        }

        public virtual void Close()
        {
            this._parent.Close();
        }

        public virtual void Dispose()
        {
            this._parent.Dispose();
        }

        public bool GetBoolean(int i)
        {
            return this._parent.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return this._parent.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._parent.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return this._parent.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._parent.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        protected virtual Func<object, object> GetConverter(DbMetadataColumn col)
        {
            if (col.Encrypt)
            {
                return new Func<object, object>(OrmTransactionBase.Decode);
            }
            return null;
        }

        private ConverterPair[] GetConverters(DbMetadataTable tableSchema)
        {
            List<ConverterPair> list = new List<ConverterPair>();
            foreach (DbMetadataColumn column in tableSchema.Columns)
            {
                Func<object, object> converter = this.GetConverter(column);
                if (converter != null)
                {
                    ConverterPair item = new ConverterPair
                    {
                        ColumnIndex = column.ColumnIndex,
                        Converter = converter
                    };
                    list.Add(item);
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            return null;
        }

        public IDataReader GetData(int i)
        {
            return this._parent.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return this._parent.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return this._parent.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return this._parent.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return this._parent.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return this._parent.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return this._parent.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return this._parent.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return this._parent.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return this._parent.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return this._parent.GetInt64(i);
        }

        public string GetName(int i)
        {
            return this._parent.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return this._parent.GetOrdinal(name);
        }

        public DataTable GetSchemaTable()
        {
            return this._parent.GetSchemaTable();
        }

        public string GetString(int i)
        {
            return this._parent.GetString(i);
        }

        public object GetValue(int i)
        {
            return this._parent.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            int num = this._parent.GetValues(values);
            if (this._converters != null)
            {
                for (int i = 0; i < this._converters.Length; i++)
                {
                    ConverterPair pair = this._converters[i];
                    values[pair.ColumnIndex] = pair.Converter(values[pair.ColumnIndex]);
                }
            }
            return num;
        }

        public bool IsDBNull(int i)
        {
            return this._parent.IsDBNull(i);
        }

        public virtual bool NextResult()
        {
            return this._parent.NextResult();
        }

        public bool Read()
        {
            return this._parent.Read();
        }

        public static object ToArray(DbType dbType, object[] value)
        {
            switch (dbType)
            {
                case DbType.Int16:
                    {
                        short[] numArray = new short[value.Length];
                        for (int i = 0; i < value.Length; i++)
                        {
                            numArray[i] = Convert.ToInt16(value[i]);
                        }
                        return numArray;
                    }
                case DbType.Int32:
                    {
                        int[] numArray2 = new int[value.Length];
                        for (int j = 0; j < value.Length; j++)
                        {
                            numArray2[j] = Convert.ToInt32(value[j]);
                        }
                        return numArray2;
                    }
                case DbType.Int64:
                    {
                        long[] numArray3 = new long[value.Length];
                        for (int k = 0; k < value.Length; k++)
                        {
                            numArray3[k] = Convert.ToInt64(value[k]);
                        }
                        return numArray3;
                    }
                case DbType.Object:
                case DbType.SByte:
                case DbType.Single:
                    return value;

                case DbType.String:
                case DbType.AnsiString:
                    {
                        string[] strArray = new string[value.Length];
                        for (int m = 0; m < value.Length; m++)
                        {
                            strArray[m] = Convert.ToString(value[m]);
                        }
                        return strArray;
                    }
            }
            return value;
        }

        public static object ToBoolean(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToBoolean(value);
        }

        public static object ToByte(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToByte(value);
        }

        public static object ToChar(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToChar(value);
        }

        public static object ToDecimal(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToDecimal(value);
        }

        public static object ToDouble(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToDouble(value);
        }

        public static object ToGuid(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            byte[] b = value as byte[];
            if (b != null)
            {
                return new Guid(b);
            }
            string g = value as string;
            if (g == null)
            {
                throw new InvalidCastException(string.Format("无法将{0}转换为GUID类型", value));
            }
            return new Guid(g);
        }

        public static object ToInt16(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToInt16(value);
        }

        public static object ToInt32(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToInt32(value);
        }

        public static object ToInt64(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToInt64(value);
        }

        public static object ToSingle(object value)
        {
            if (value == DBNull.Value)
            {
                return DBNull.Value;
            }
            return Convert.ToSingle(value);
        }

 
        public int Depth
        {
            get
            {
                return this._parent.Depth;
            }
        }

        public int FieldCount
        {
            get
            {
                return this._parent.FieldCount;
            }
        }

        public bool IsClosed
        {
            get
            {
                return this._parent.IsClosed;
            }
        }

        public object this[string name]
        {
            get
            {
                return this._parent[name];
            }
        }

        public object this[int i]
        {
            get
            {
                return this._parent[i];
            }
        }

        public IDataReader Parent
        {
            get
            {
                return this._parent;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return this._parent.RecordsAffected;
            }
        }

        public DbMetadataTable TableSchema
        {
            get
            {
                return this._tableSchema;
            }
        }

 
        [StructLayout(LayoutKind.Sequential)]
        private struct ConverterPair
        {
            public int ColumnIndex;
            public Func<object, object> Converter;
        }
    }
}
