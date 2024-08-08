using Antlr4.Runtime.Misc;
using WYF.Bos.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.dataentity
{
    public class SelectSql
    {
        public string selectSql { get; set; }

        public String SelectWhere { get; set; }

        public List<SqlParameter> SelectParams { get; set; } = new List<SqlParameter>();

        public String CountSql { get; set; }

        public String CountWhere { get; set; }

        public String CountGroupBySqlpart;

        public List<SqlParameter> CountParams { get; set; } = new List<SqlParameter>();

        public SqlBuilder SelectSqlBuild { get; set; }

        public SqlBuilder CountSqlBuild { get; set; }
    }
}
