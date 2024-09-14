using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    public class JavaObjectValueExpr : SqlExpr
    {
        // Fields
        public object value;

        // Methods
        public JavaObjectValueExpr() : base(0x1a)
        {
        }

        public JavaObjectValueExpr(object value) : base(0x1a)
        {
            this.value = value;
        }

        public override object Clone()
        {
            return new JavaObjectValueExpr(this.value);
        }
    }






}
