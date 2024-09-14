using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlAllColumnExpr : SqlExpr
    {
        // Methods
        public SqlAllColumnExpr() : base(8)
        {
        }

        public override object Clone()
        {
            return new SqlAllColumnExpr();
        }

        public override void output(StringBuilder buff)
        {
            buff.Append('*');
        }
    }






}
