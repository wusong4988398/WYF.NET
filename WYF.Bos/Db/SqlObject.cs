using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public class SqlObject
    {
    
        public  StringBuilder Sql { get; set; } = new StringBuilder();
        public List<SqlParameter> Parameters { get; set; } = new List<SqlParameter>();


        public SqlObject(String sql, SqlParameter[] parameters)
        {
            SetSql(sql);
            SetParams (parameters);
        }
        public  void SetSql(String sql)
        {
            this.Sql.Length = 0;
            if (Sql != null)
                this.Sql.Append(sql);
        }
        public  void SetParams(SqlParameter[] parameters)
        {
            this.Parameters.Clear();
            if (parameters != null && parameters.Length > 0)
            this.Parameters.AddRange(parameters.ToList());
        }
    }
}
