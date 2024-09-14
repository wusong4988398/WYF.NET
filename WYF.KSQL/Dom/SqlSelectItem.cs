using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlSelectItem : SqlObject
    {
        // Fields
        public string alias;
        private string asWord;
        public SqlExpr expr;
        private bool isequalMark;
        private string orgAliasWord;

        // Methods
        public SqlSelectItem()
        {
            this.asWord = "";
        }

        public SqlSelectItem(SqlExpr expr, string alias)
        {
            this.asWord = "";
            this.expr = expr;
            this.alias = alias;
            this.setOrgAliasWord(alias);
        }

        public SqlSelectItem(SqlExpr expr, string alias, string orgAlias)
        {
            this.asWord = "";
            this.expr = expr;
            this.alias = alias;
            this.setAsWord("");
            this.setOrgAliasWord(orgAlias);
        }

        public SqlSelectItem(SqlExpr expr, string alias, string orgAlias, string asWord)
        {
            this.asWord = "";
            this.expr = expr;
            this.alias = alias;
            this.setAsWord(asWord);
            this.setOrgAliasWord(orgAlias);
        }

        public SqlSelectItem(SqlExpr expr, string alias, string orgAlias, string asWord, bool mark)
        {
            this.asWord = "";
            this.expr = expr;
            this.alias = alias;
            this.setAsWord(asWord);
            this.setOrgAliasWord(orgAlias);
            this.setEqualMark(mark);
        }

        public override object Clone()
        {
            if (this.expr != null)
            {
                return new SqlSelectItem((SqlExpr)this.expr.Clone(), this.alias, this.orgAliasWord, this.asWord, this.isequalMark);
            }
            return new SqlSelectItem(null, this.alias, this.orgAliasWord, this.asWord, this.isequalMark);
        }

        public string getAsWord()
        {
            return this.asWord;
        }

        public string getOrgAliasWord()
        {
            if ((this.orgAliasWord != null) && (this.orgAliasWord.Trim().Length != 0))
            {
                return this.orgAliasWord;
            }
            return this.alias;
        }

        public bool isEqualMark()
        {
            return this.isequalMark;
        }

        public void output(StringBuilder buff)
        {
            if (this.expr != null)
            {
                this.expr.output(buff);
            }
            else
            {
                buff.Append("null");
            }
            if ((this.alias != null) && (this.alias.Length != 0))
            {
                buff.Append(' ');
                buff.Append(this.alias);
            }
        }

        public void setAsWord(string asWord)
        {
            this.asWord = asWord;
        }

        public void setEqualMark(bool isEqualMark)
        {
            this.isequalMark = isEqualMark;
        }

        public void setOrgAliasWord(string orgAliasWord)
        {
            this.orgAliasWord = orgAliasWord;
        }

        public string toString()
        {
            StringBuilder buff = new StringBuilder();
            this.output(buff);
            return buff.ToString();
        }
    }


 


}
