using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlDefaultExpr : SqlExpr
    {
        // Fields
        public string text;

        // Methods
        public SqlDefaultExpr() : base(-1)
        {
        }

        public SqlDefaultExpr(string text) : base(-1)
        {
            this.text = text;
        }

        public override object Clone()
        {
            return new SqlDefaultExpr(this.text);
        }
    }


    



}
