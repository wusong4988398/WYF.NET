using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.xdb.sharding.sql.parser
{
    public class SQLUtil
    {
        public static string UnWrapSQLTableName(string tableName)
        {
            char ch = tableName.ElementAt(0);
            if (ch == '\'' || ch == '"' || ch == '`')
            {
                if (tableName.ElementAt(tableName.Length - 1) == ch)
                    return tableName.Substring(1, tableName.Length - 1);
                throw new ArgumentException("表名不规范:"+ tableName);
            }
            return tableName;
        }
    }
}
