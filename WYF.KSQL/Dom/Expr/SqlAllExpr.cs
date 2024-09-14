using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public sealed class SqlAllExpr : SqlExpr
    {
        // Fields
        public SqlSelectBase subQuery;

        // Methods
        public SqlAllExpr() : base(0x10)
        {
            this.setExprWord("ALL");
        }

        public SqlAllExpr(SqlSelectBase subQuery) : base(0x10)
        {
            this.subQuery = subQuery;
            this.setExprWord("ALL");
        }

        public SqlAllExpr(string orgWord, SqlSelectBase subQuery) : base(0x10)
        {
            this.subQuery = subQuery;
            this.setExprWord(orgWord);
        }

        public override object Clone()
        {
            SqlAllExpr expr = new SqlAllExpr();
            if (this.subQuery != null)
            {
                expr.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            expr.setExprWord(this.getExprWord());
            return expr;
        }
    }





}
