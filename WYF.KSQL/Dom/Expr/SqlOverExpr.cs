using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlOverExpr : SqlExpr
    {
        // Fields
        public List<SqlOrderByItem> orderBy;
        public List<SqlExpr> partition;

        // Methods
        public SqlOverExpr() : base(4)
        {
            this.partition = new List<SqlExpr>();
            this.orderBy = new List<SqlOrderByItem>();
        }

        public override string toString()
        {
            return base.toString();
        }
    }






}
