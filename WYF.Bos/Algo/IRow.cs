using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo
{
    public interface IRow : IRowFeature
    {
        int Size();



        object Get(string columnName);

        string GetString(int index);

        string GetString(string columnName);

        int? GetInteger(int index);

        int? GetInteger(string columnName);

        long? GetLong(int index);

        long? GetLong(string columnName);

        bool? GetBoolean(int index);

        bool? GetBoolean(string columnName);

        double? GetDouble(int index);

        double? GetDouble(string columnName);

        decimal? GetDecimal(int index);

        decimal? GetDecimal(string columnName);

        DateTime? GetDate(int index);

        DateTime? GetDate(string columnName);

        DateTime? GetTimestamp(int index);

        DateTime? GetTimestamp(string columnName);
    }
}
