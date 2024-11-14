using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine;
using WYF.DbEngine.db;

namespace WYF.OrmEngine.DataEntity
{
    public class SelectSql
    {
        public string selectSql { get; set; }

        public string SelectWhere { get; set; }

        public List<SqlParam> SelectParams { get; set; } = new List<SqlParam>();

        public string CountSql { get; set; }

        public string CountWhere { get; set; }

        public string CountGroupBySqlpart;

        public List<SqlParam> CountParams { get; set; } = new List<SqlParam>();

        public SqlBuilder SelectSqlBuild { get; set; }

        public SqlBuilder CountSqlBuild { get; set; }
    }
}
