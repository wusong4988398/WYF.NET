using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlNotExpr : SqlExpr
    {
        // Fields
        public SqlExpr expr;

        // Methods
        public SqlNotExpr() : base(9)
        {
        }

        public SqlNotExpr(SqlExpr expr) : base(9)
        {
            this.expr = expr;
        }

        public override object Clone()
        {
            SqlNotExpr expr = new SqlNotExpr();
            if (expr != null)
            {
                expr.expr = (SqlExpr)this.expr.Clone();
            }
            return expr;
        }
    }





}
