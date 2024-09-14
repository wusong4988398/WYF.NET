using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlVarRefExpr : SqlExpr
    {
        // Fields
        public string text;

        // Methods
        public SqlVarRefExpr() : base(3)
        {
        }

        public SqlVarRefExpr(string text) : base(3)
        {
            this.text = text;
        }

        public override object Clone()
        {
            return new SqlVarRefExpr(this.text);
        }
    }


   



}
