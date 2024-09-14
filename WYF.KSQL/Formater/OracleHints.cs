using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;

namespace WYF.KSQL.Formater
{
    public abstract class OracleHints : Hints
    {
        // Fields
        protected Hashtable hints = new Hashtable();

        // Methods
        protected OracleHints()
        {
            this.NewHint("ALL_ROWS");
            this.NewHint("APPEND", 8, 0, 0);
            this.NewHint("BYPASS_UJVC");
            this.NewHint("CACHE", 1, -1);
            this.NewHint("CLUSTER", 1, -1);
            this.NewHint("CURSOR_SHARING_EXACT");
            this.NewHint("DRIVING_SITE", 1, -1);
            this.NewHint("DYNAMIC_SAMPLING", 1, -1);
            this.NewHint("FACT", 1, -1);
            this.NewHint("FIRST_ROWS", 1, 1);
            this.NewHint("FULL", 1, -1);
            this.NewHint("HASH", 1, -1);
            this.NewHint("INDEX", 1, -1);
            this.NewHint("INDEX_ASC", 1, -1);
            this.NewHint("INDEX_COMBINE", 1, -1);
            this.NewHint("INDEX_DESC", 1, -1);
            this.NewHint("INDEX_FFS", 1, -1);
            this.NewHint("INDEX_JOIN", 1, -1);
            this.NewHint("INDEX_SS", 1, -1);
            this.NewHint("INDEX_SS_ASC", 1, -1);
            this.NewHint("INDEX_SS_DESC", 1, -1);
            this.NewHint("LEADING", 1, -1);
            this.NewHint("MERGE", 0x11, 0, -1);
            this.NewHint("MODEL_MIN_ANALYSIS");
            this.NewHint("NOAPPEND");
            this.NewHint("NOCACHE", 1, -1);
            this.NewHint("NO_EXPAND", 0, 1);
            this.NewHint("NO_FACT", 1, -1);
            this.NewHint("NO_INDEX", 1, -1);
            this.NewHint("NO_INDEX_FFS", 1, -1);
            this.NewHint("NO_INDEX_SS", 1, -1);
            this.NewHint("NO_MERGE", 0, -1);
            this.NewHint("NO_PARALLEL_INDEX", 1, -1);
            this.NewHint("NO_PUSH_PRED", 0, -1);
            this.NewHint("NO_PUSH_SUBQ", 0, 1);
            this.NewHint("NO_PX_JOIN_FILTER", 1, -1);
            this.NewHint("NO_REWRITE", 0, 1);
            this.NewHint("NO_QUERY_TRANSFORMATION");
            this.NewHint("NO_STAR_TRANSFORMATION", 0, 1);
            this.NewHint("NO_UNNEST", 0, 1);
            this.NewHint("NO_USE_HASH", 1, -1);
            this.NewHint("NO_USE_MERGE", 1, -1);
            this.NewHint("NO_USE_NL", 1, -1);
            this.NewHint("NO_XML_QUERY_REWRITE");
            this.NewHint("ORDERED");
            this.NewHint("PARALLEL", 0x1f, 1, -1);
            this.NewHint("PARALLEL_INDEX", 1, -1);
            this.NewHint("PUSH_PRED", 1, -1);
            this.NewHint("PX_JOIN_FILTER", 1, -1);
            this.NewHint("QB_NAME", 1, 1);
            this.NewHint("REWRITE", 0, -1);
            this.NewHint("STAR_TRANSFORMATION", 0, 1);
            this.NewHint("UNNEST", 0, 1);
            this.NewHint("USE_CONCAT", 0, 1);
            this.NewHint("USE_HASH", 1, -1);
            this.NewHint("USE_MERGE", 1, -1);
            this.NewHint("USE_NL", 1, -1);
            this.NewHint("USE_NL_WITH_INDEX");
        }

        public bool CheckHint(KHint hint, StringBuilder sb)
        {
            if (hint == null)
            {
                sb.Append("Hint is null");
                return false;
            }
            string str = hint.getName().ToUpper();
            OraHint hint1 = (OraHint)this.hints[str];
            OraHint hint2 = (OraHint)this.hints[str];
            if (hint2 == null)
            {
                sb.Append("Not supported oracle hint : " + hint.getName());
                return false;
            }
            int count = hint.getParameters().Count;
            if ((count > hint2.maxParamSize) && (hint2.maxParamSize >= 0))
            {
                sb.Append(string.Concat(new object[] { "Parameter size Eorror for hint:", hint.getName(), " Expect max parameters size is: ", hint2.maxParamSize, ", but found ", count, " parameters" }));
                return false;
            }
            if (count < hint2.minParamSize)
            {
                sb.Append(string.Concat(new object[] { "Parameter size Eorror for hint:", hint.getName(), ".Expect min parameters size is: ", hint2.minParamSize, ", but found ", count, " parameters" }));
                return false;
            }
            return true;
        }

        public string FormatHints(IList hints, SqlObject sql)
        {
            if ((hints == null) || (hints.Count == 0))
            {
                return "";
            }
            StringBuilder buffer = new StringBuilder();
            buffer.Append("/*+ ");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hints.Count; i++)
            {
                KHint hint = (KHint)hints[i];
                if (!this.CheckHint(hint, sb))
                {
                    throw new FormaterException(sb.ToString());
                }
                this.Output(hint, buffer);
                buffer.Append(" ");
            }
            buffer.Append("*/ ");
            return buffer.ToString();
        }

        public void FormatHints(IList hints, SqlObject sql, StringBuilder buffer)
        {
            buffer.Append(this.FormatHints(hints, sql));
        }

        protected void NewHint(string name)
        {
            this.NewHint(name, name, 1, 0, 0);
        }

        protected void NewHint(string name, int minParamSize, int maxParamSize)
        {
            this.NewHint(name, name, 1, minParamSize, maxParamSize);
        }

        protected void NewHint(string name, int type, int minParamSize, int maxParamSize)
        {
            OraHint hint = new OraHint(name, name, type, minParamSize, maxParamSize);
            this.hints.Add(name, hint);
        }

        protected void NewHint(string name, string value, int type, int minParamSize, int maxParamSize)
        {
            OraHint hint = new OraHint(name, value, type, minParamSize, maxParamSize);
            this.hints.Add(name, hint);
        }

        public void Output(KHint hint, StringBuilder buffer)
        {
            this.Output(hint, buffer, true);
        }

        public void Output(KHint hint, StringBuilder buffer, bool flag)
        {
            OraHint hint2 = (OraHint)this.hints[hint.getName().ToUpper()];
            if (flag)
            {
                buffer.Append(hint2.value);
            }
            else
            {
                buffer.Append(hint.getOrgName());
            }
            if ((hint.getParameters() != null) && (hint.getParameters().Count != 0))
            {
                buffer.Append("(");
                for (int i = 0; i < hint.getParameters().Count; i++)
                {
                    if (i > 0)
                    {
                        buffer.Append(" ");
                    }
                    buffer.Append(hint.getParameters()[i].ToString().Trim());
                }
                buffer.Append(")");
            }
        }

        // Nested Types
        private class OraHint
        {
            // Fields
            public int maxParamSize = -1;
            public int minParamSize;
            public string name;
            public int type = 1;
            public string value;

            // Methods
            public OraHint(string name, string value, int type, int minParamSize, int maxParamSize)
            {
                this.name = name.ToUpper();
                this.value = value.ToUpper();
                this.type = type;
                this.minParamSize = minParamSize;
                this.maxParamSize = maxParamSize;
            }
        }
    }


    



}
