using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Community.CsharpSqlite.Sqlite3;

namespace WYF.Algo.dataset
{
    public class ResultSetRowImpl : AbstractRow
    {
        private  RowMeta rowMeta;
        private  IDataReader rs;
        private  int[] dataTypeOrdinals;
        public ResultSetRowImpl(RowMeta rowMeta, IDataReader rs)
        {
            this.dataTypeOrdinals = rowMeta.GetDataTypeOrdinals();
            this.rowMeta = rowMeta;
            this.rs = rs;
        }

        public override int Size => this.dataTypeOrdinals.Length;

        public override object Get(int index)
        {
            return GetObject(this.rs, this.dataTypeOrdinals[index], index, this.rowMeta.IsNullable(index));
        }

        public override RowMeta GetRowMeta()
        {
            return this.rowMeta;
        }

        public override object[] Values()
        {
            return DefaultConvertValues();
        }
        protected object[] DefaultConvertValues()
        {
            int len = this.Size;
            Object[] values = new Object[len];
            for (int i = 0; i < len; i++)
                values[i] = Get(i);
            return values;
        }
        private object GetObject(IDataReader rs, int ordinal, int index, bool nullable)
        {
            switch (ordinal)
            {
                case 0:
                    return rs.GetBoolean(index);
                case 2:
                    if (nullable)
                    {
                        var value = rs.GetValue(index);
                        if (value == DBNull.Value)
                            return null;
                        if (value is IConvertible)
                            return Convert.ToInt32(value);
                    }
                    return rs.GetInt32(index);
                case 3:
                    if (nullable)
                    {
                        var value = rs.GetValue(index);
                        if (value == DBNull.Value)
                            return null;
                        if (value is IConvertible)
                            return Convert.ToInt64(value);
                    }
                    return rs.GetInt64(index);
                case 5:
                    return rs.GetDecimal(index);
                case 1:
                    return rs.GetString(index);
                case 6:
                    return rs.GetDateTime(index).Date;
                case 7:
                    return rs.GetDateTime(index);
                case 4:
                    if (nullable)
                    {
                        var value = rs.GetValue(index);
                        if (value == DBNull.Value)
                            return null;
                        if (value is IConvertible)
                            return Convert.ToDouble(value);
                    }
                    return rs.GetDouble(index);
                default:
                    throw new NotSupportedException($"Unimplemented for data type: {ordinal}");
            }
        }

    }
}
