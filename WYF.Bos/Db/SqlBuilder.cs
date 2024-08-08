using Antlr4.Runtime.Misc;
using WYF.Bos.db.tx;
using WYF.Bos.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public class SqlBuilder
    {
        public List<Seg> Segs { get; set; } = new List<Seg>();

        public SqlBuilder Append(String sqlPart, List<Object> paramters)
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

        public SqlBuilder AppendIn(String field, Object[] inValues)
        {
            Seg seg = new Seg();
            seg.IsIn = true;
            if (StringUtils.IsEmpty(field))
            {
                seg.SqlPart = "";
            }
            else
            {
                seg.SqlPart = " " + field;
            }
            seg.Paramters = inValues;
            this.Segs.Add(seg);
            return this;
        }
        public SqlObject GenSQLObject(DelegateConnection con)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Seg seg in this.Segs)
            {
                seg.Gen(con);
                if (seg.SqlPart[0] != ' ')
                    sb.Append(' ');
                sb.Append(seg.SqlPart);
            }
            List<object> pList = GetParams();
            SqlParameter[] ps = new SqlParameter[pList.Count];
            int i = 0;
            foreach (object p in pList)
                ps[i++] = new SqlParameter(p);
            return new SqlObject(sb.ToString(), ps);
        }

        private List<object> GetParams()
        {
            List<object> list = new List<object>();
            foreach (Seg seg in this.Segs)
            {
                object[] ps = seg.Paramters;
                if (ps != null)
                {
                    foreach (object p in ps)
                    {
                        list.Add(p);
                    }
                }
            }
            return list;
        }
        public class Seg
        {
            public bool IsIn { get; set; }

            public bool IsInGenned { get; set; }

            public string SqlPart { get; set; }

            public object[] Paramters { get;set; }
    
            public Seg() { }

            internal void Gen(DelegateConnection con)
            {
                if (!this.IsIn || this.IsInGenned)
                    return;
                bool isCreateTemp = false;
                if (!isCreateTemp)
                {
                    StringBuilder stringBuilder = new StringBuilder(this.SqlPart);
                    stringBuilder.Append(" IN (");
                    for (int i = 0; i < this.Paramters.Length; i++) {
                        if (i == 0)
                        {
                            stringBuilder.Append('?');
                        }
                        else
                        {
                            stringBuilder.Append(",?");
                        }
                    }
                    stringBuilder.Append(')');
                    this.SqlPart = stringBuilder.ToString();
                }
                this.IsInGenned = true;
            }
        }
    }
}
