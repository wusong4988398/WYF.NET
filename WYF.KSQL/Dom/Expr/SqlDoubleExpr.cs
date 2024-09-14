using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlDoubleExpr : SqlExpr
    {
        // Fields
        public string text;
        public double value;

        // Methods
        public SqlDoubleExpr() : base(2)
        {
        }

        public SqlDoubleExpr(double value) : base(2)
        {
            this.value = value;
            this.text = value.ToString();
        }

        public SqlDoubleExpr(string text) : base(2)
        {
            this.text = text;
            this.value = double.Parse(text);
        }

        public override object Clone()
        {
            return new SqlDoubleExpr { value = this.value, text = this.text };
        }
    }


 



}
