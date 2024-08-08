using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    public class QParameter
    {
        private string sql;
        public object[] Parameters { get; private set; }



        public  QContext Ctx { get; set; }

        public QFilter MatchTransferQFilter { get; set; }
        private Func<string, QContext, string> _sqlReBuilder;

        public QParameter(string partSql, params object[] parameters)
        {
            this.sql = partSql;
            this.Parameters = parameters;
        }
        public void SetSQLBuilder(Func<string, QContext, string> sqlReBuilder)
        {

            this._sqlReBuilder = sqlReBuilder;
        }
        public string GetSql()
        {
            if (this._sqlReBuilder != null)
                return this._sqlReBuilder.Invoke(this.sql, this.Ctx);
            return this.sql;
        }
    }

  
}
