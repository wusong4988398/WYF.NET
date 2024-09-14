using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    public class SqlConvertTypeExpr : SqlIdentifierExpr
    {
        // Fields
        private int len;

        // Methods
        public SqlConvertTypeExpr()
        {
        }

        public SqlConvertTypeExpr(string value) : base(value)
        {
        }

        public SqlConvertTypeExpr(string value, int len) : base(value)
        {
            this.len = len;
        }

        public override object Clone()
        {
            return new SqlConvertTypeExpr(base.value, this.len);
        }

        public int getLen()
        {
            return this.len;
        }

        public void setLen(int len)
        {
            this.len = len;
        }
    }






}
