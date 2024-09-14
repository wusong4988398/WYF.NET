using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlLongExpr : SqlExpr
    {
        // Fields
        public string text;
        public long value;

        // Methods
        public SqlLongExpr() : base(0x1b)
        {
        }

        public SqlLongExpr(int value) : base(0x1b)
        {
            this.text = value.ToString();
            this.value = value;
        }

        public SqlLongExpr(long value) : base(0x1b)
        {
            this.text = value.ToString();
            this.value = long.Parse(this.text);
        }

        public SqlLongExpr(string text) : base(0x1b)
        {
            this.text = text;
            this.value = long.Parse(text);
        }

        public override object Clone()
        {
            return new SqlLongExpr { text = this.text, value = this.value };
        }
    }






}
