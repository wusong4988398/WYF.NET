using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlPriorIdentifierExpr : SqlExpr
    {
        // Fields
        public string value;

        // Methods
        public SqlPriorIdentifierExpr() : base(0x19)
        {
        }

        public SqlPriorIdentifierExpr(string value) : base(0x19)
        {
            this.value = value;
        }

        public override object Clone()
        {
            return new SqlPriorIdentifierExpr(this.value);
        }
    }






}
