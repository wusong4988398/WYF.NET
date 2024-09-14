using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class QueryExpr : SqlExpr
    {
        // Fields
        public SqlSelectBase subQuery;

        // Methods
        public QueryExpr() : base(0x18)
        {
        }

        public QueryExpr(SqlSelectBase select) : base(0x18)
        {
            this.subQuery = select;
        }

        public override object Clone()
        {
            QueryExpr expr = new QueryExpr();
            if (this.subQuery != null)
            {
                expr.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            return expr;
        }
    }


   



}
