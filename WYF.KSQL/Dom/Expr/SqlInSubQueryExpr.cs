using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlInSubQueryExpr : SqlExpr
    {
        // Fields
        public SqlExpr expr;
        public bool not;
        public SqlSelectBase subQuery;

        // Methods
        public SqlInSubQueryExpr() : base(13)
        {
        }

        public SqlInSubQueryExpr(SqlExpr expr, SqlSelectBase subQuery) : base(13)
        {
            this.expr = expr;
            this.subQuery = subQuery;
            this.setExprWord("IN");
        }

        public SqlInSubQueryExpr(SqlExpr expr, SqlSelectBase subQuery, bool not) : base(13)
        {
            this.expr = expr;
            this.subQuery = subQuery;
            this.not = not;
            if (this.not)
            {
                this.setExprWord("NOT IN");
            }
            else
            {
                this.setExprWord("IN");
            }
        }

        public override object Clone()
        {
            SqlInSubQueryExpr expr = new SqlInSubQueryExpr();
            if (this.expr != null)
            {
                expr.expr = (SqlExpr)this.expr.Clone();
            }
            if (this.subQuery != null)
            {
                expr.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            expr.not = this.not;
            expr.setExprWord(this.getExprWord());
            return expr;
        }
    }


  



}
