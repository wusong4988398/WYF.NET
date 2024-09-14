using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlAnyExpr : SqlExpr
    {
        // Fields
        public SqlSelectBase subQuery;

        // Methods
        public SqlAnyExpr() : base(0x12)
        {
            this.setExprWord("ANY");
        }

        public SqlAnyExpr(SqlSelectBase subQuery) : base(0x12)
        {
            this.subQuery = subQuery;
            this.setExprWord("ANY");
        }

        public SqlAnyExpr(string orgWord, SqlSelectBase subQuery) : base(0x12)
        {
            this.subQuery = subQuery;
            this.setExprWord(orgWord);
        }

        public override object Clone()
        {
            SqlAnyExpr expr = new SqlAnyExpr();
            if (this.subQuery != null)
            {
                expr.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            expr.setExprWord(this.getExprWord());
            return expr;
        }
    }


  



}
