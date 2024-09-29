using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo
{
    public interface IRow
    {
        int Size { get; }

        object this[int index] { get; }
        object this[string columnName] { get; }

        string GetString(int index);
        string GetString(string columnName);

        int? GetInt32(int index);
        int? GetInt32(string columnName);

        long? GetInt64(int index);
        long? GetInt64(string columnName);

        bool? GetBoolean(int index);
        bool? GetBoolean(string columnName);

        double? GetDouble(int index);
        double? GetDouble(string columnName);

        decimal? GetDecimal(int index);
        decimal? GetDecimal(string columnName);

        DateTime? GetDateTime(int index);
        DateTime? GetDateTime(string columnName);

        DateTimeOffset? GetTimestamp(int index);
        DateTimeOffset? GetTimestamp(string columnName);
    }

}
