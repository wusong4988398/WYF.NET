using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.DbEngine.db
{
    public class SqlBuilder
    {
        public List<Seg> Segs { get; set; } = new List<Seg>();

        public bool IsEmpty=> Segs.Count == 0;
        
   

        public SqlBuilder Append(string sqlPart, List<object> paramters)
        {
            return Append(sqlPart, paramters.ToArray());
        }

        public SqlBuilder Append(string sqlPart, params object[] paramters)
        {
            Seg seg = new Seg();
            seg.SqlPart = sqlPart;
            seg.Paramters = paramters;
            this.Segs.Add(seg);
            return this;
        }
        public SqlBuilder AppendSqlBuilder(SqlBuilder sqlBuilder)
        {
            if (sqlBuilder != null)
                this.Segs.AddRange(sqlBuilder.Segs);
            return this;
        }
        public SqlBuilder AppendIn(string field, object[] inValues)
        {
            Seg seg = new Seg();
            seg.IsIn = true;
            seg.SqlPart = string.IsNullOrEmpty(field) ? "" : " " + field;
            seg.Paramters = inValues;
            this.Segs.Add(seg);
            return this;
        }
    }
}
