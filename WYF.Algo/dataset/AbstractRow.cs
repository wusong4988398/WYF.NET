using IronPython.Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.Algo.dataset
{
    public abstract class AbstractRow : IRow
    {


        public abstract int Size { get; }

        public abstract object Get(int index);
       

        public object Get(string field)
        {
            return Get(field, true);
        }
        public object Get(string field, bool exception)
        {
            int index = GetRowMeta().GetFieldIndex(field);
            if (index < 0)
            {
                if (exception)
                    throw new AlgoException("Field " + field + " not found.");
                return null;
            }
            return Get(index);
        }

        public bool? GetBoolean(int index)
        {
            throw new NotImplementedException();
        }

        public bool? GetBoolean(string columnName)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetDateTime(int index)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetDateTime(string columnName)
        {
            throw new NotImplementedException();
        }

        public decimal? GetDecimal(int index)
        {
            throw new NotImplementedException();
        }

        public decimal? GetDecimal(string columnName)
        {
            throw new NotImplementedException();
        }

        public double? GetDouble(int index)
        {
            throw new NotImplementedException();
        }

        public double? GetDouble(string columnName)
        {
            throw new NotImplementedException();
        }

        public int? GetInt32(int index)
        {
            throw new NotImplementedException();
        }

        public int? GetInt32(string columnName)
        {
            throw new NotImplementedException();
        }

        public long? GetInt64(int index)
        {
            throw new NotImplementedException();
        }

        public long? GetInt64(string columnName)
        {
            throw new NotImplementedException();
        }

        public string GetString(int index)
        {
            object v = Get(index);
            if (v == null)
                return null;
            if (v is string) return (string)v;
            return (string)DataType.ConvertValue((DataType)DataType.StringType, v);
        }

        public string GetString(string field)
        {
            object v = Get(field);
            if (v == null)
                return null;
            if (v is string) return (string)v;
            return (string)DataType.ConvertValue((DataType)DataType.StringType, v);
        }

        public DateTimeOffset? GetTimestamp(int index)
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset? GetTimestamp(string columnName)
        {
            throw new NotImplementedException();
        }

        public abstract RowMeta GetRowMeta();
        public abstract object[] Values();
    }
}
