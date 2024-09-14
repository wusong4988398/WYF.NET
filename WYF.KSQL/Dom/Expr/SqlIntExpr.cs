using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlIntExpr : SqlExpr
    {
        // Fields
        public string text;
        public int value;

        // Methods
        public SqlIntExpr() : base(1)
        {
        }

        public SqlIntExpr(int value) : base(1)
        {
            this.text = value.ToString();
            this.value = value;
        }

        public SqlIntExpr(string text) : base(1)
        {
            this.text = text;
            this.value = int.Parse(text);
        }

        public override object Clone()
        {
            return new SqlIntExpr { text = this.text, value = this.value };
        }
    }





}
