using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlCaseExpr : SqlExpr
    {
        // Fields
        public SqlExpr elseExpr;
        private string elseWord;
        private string endWord;
        public ArrayList itemList;
        public SqlExpr valueExpr;

        // Methods
        public SqlCaseExpr() : base(12)
        {
            this.itemList = new ArrayList();
        }

        public override object Clone()
        {
            SqlCaseExpr expr = new SqlCaseExpr();
            if (this.itemList != null)
            {
                for (int i = 0; i < this.itemList.Count; i++)
                {
                    SqlCaseItem item = (SqlCaseItem)((SqlCaseItem)this.itemList[i]).Clone();
                    expr.itemList.Add(item);
                }
            }
            if (this.valueExpr != null)
            {
                expr.valueExpr = (SqlExpr)this.valueExpr.Clone();
            }
            if (this.elseExpr != null)
            {
                expr.elseExpr = (SqlExpr)this.elseExpr.Clone();
            }
            expr.setExprWord(this.getExprWord());
            return expr;
        }

        public string getElseWord()
        {
            if ((this.elseWord != null) && (this.elseWord.Trim().Length != 0))
            {
                return this.elseWord;
            }
            return "ELSE";
        }

        public string getEndWord()
        {
            if ((this.endWord != null) && (this.endWord.Trim().Length != 0))
            {
                return this.endWord;
            }
            return "END";
        }

        public void setElseWord(string elseWord)
        {
            this.elseWord = elseWord;
        }

        public void setEndWord(string endWord)
        {
            this.endWord = endWord;
        }
    }


  


}
