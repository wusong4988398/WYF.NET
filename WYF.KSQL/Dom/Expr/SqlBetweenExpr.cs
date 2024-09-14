using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlBetweenExpr : SqlExpr
    {
        // Fields
        private string andWord;
        public SqlExpr beginExpr;
        public SqlExpr endExpr;
        public bool not;
        public SqlExpr testExpr;

        // Methods
        public SqlBetweenExpr() : base(0x11)
        {
            this.andWord = "AND";
            this.setExprWord("BETWEEN");
        }

        public SqlBetweenExpr(SqlExpr testExpr, SqlExpr beginExpr, SqlExpr endExpr) : base(0x11)
        {
            this.andWord = "AND";
            this.testExpr = testExpr;
            this.beginExpr = beginExpr;
            this.endExpr = endExpr;
            this.setExprWord("BETWEEN");
        }

        public SqlBetweenExpr(SqlExpr testExpr, SqlExpr beginExpr, SqlExpr endExpr, bool not) : base(0x11)
        {
            this.andWord = "AND";
            this.testExpr = testExpr;
            this.beginExpr = beginExpr;
            this.endExpr = endExpr;
            this.not = not;
            this.setExprWord("BETWEEN");
        }

        public override object Clone()
        {
            SqlBetweenExpr expr = new SqlBetweenExpr
            {
                not = this.not
            };
            if (this.testExpr != null)
            {
                expr.testExpr = (SqlExpr)this.testExpr.Clone();
            }
            if (this.beginExpr != null)
            {
                expr.beginExpr = (SqlExpr)this.beginExpr.Clone();
            }
            if (this.endExpr != null)
            {
                expr.endExpr = (SqlExpr)this.endExpr.Clone();
            }
            expr.setExprWord(this.getExprWord());
            expr.setAndWord(this.getAndWord());
            return expr;
        }

        public string getAndWord()
        {
            return this.andWord;
        }

        public void setAndWord(string andWord)
        {
            this.andWord = andWord;
        }
    }





}
