using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
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

        public SqlObject GenSQLObject()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Seg seg in Segs)
            {
                seg.Gen();
                if (seg.SqlPart.Length > 0 && seg.SqlPart[0] != ' ')
                {
                    sb.Append(' ');
                }
                sb.Append(seg.SqlPart);
            }

            List<object> pList = GetParams();
            SqlParameter[] ps = new SqlParameter[pList.Count];
            int i = 0;
            foreach (object p in pList)
            {
                ps[i++] = new SqlParameter("@" + i.ToString(), p);
                //{ ParameterName = "@" + i.ToString(), Value = p };
            }

            return new SqlObject(sb.ToString(), ps);
        }

        private List<object> GetParams()
        {
            List<object> list = new List<object>();
            foreach (Seg seg in this.Segs)
            {
                object[] ps = seg.Paramters;
                if (ps != null)
                    foreach (var p in ps)
                    {
                        list.Add(p);
                    }
            }
            return list;
        }
    }
}
