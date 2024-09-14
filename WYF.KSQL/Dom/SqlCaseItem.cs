using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlCaseItem : SqlObject
    {
        // Fields
        public SqlExpr conditionExpr;
        private string thenWord;
        public SqlExpr valueExpr;
        private string whenWord;

        // Methods
        public SqlCaseItem()
        {
        }

        public SqlCaseItem(SqlExpr conditionExpr, SqlExpr valueExpr)
        {
            this.conditionExpr = conditionExpr;
            this.valueExpr = valueExpr;
        }

        public override object Clone()
        {
            SqlCaseItem item = new SqlCaseItem();
            if (this.conditionExpr != null)
            {
                item.conditionExpr = (SqlExpr)this.conditionExpr.Clone();
            }
            if (this.valueExpr != null)
            {
                item.valueExpr = (SqlExpr)this.valueExpr.Clone();
            }
            return item;
        }

        public string getThenWord()
        {
            if ((this.thenWord != null) && (this.thenWord.Trim().Length != 0))
            {
                return this.thenWord;
            }
            return "THEN";
        }

        public string getWhenWord()
        {
            if ((this.whenWord != null) && (this.whenWord.Trim().Length != 0))
            {
                return this.whenWord;
            }
            return "WHEN";
        }

        public void setThenWord(string thenWord)
        {
            this.thenWord = thenWord;
        }

        public void setWhenWord(string whenWord)
        {
            this.whenWord = whenWord;
        }
    }






}
