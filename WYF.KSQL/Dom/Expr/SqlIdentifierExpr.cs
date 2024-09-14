using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlIdentifierExpr : SqlExpr
    {
        // Fields
        private string orgValue;
        public string value;

        // Methods
        public SqlIdentifierExpr() : base(4)
        {
        }

        public SqlIdentifierExpr(string value) : base(4)
        {
            this.value = value;
            this.setOrgValue(value);
        }

        public SqlIdentifierExpr(string value, string orgValue) : base(4)
        {
            this.value = value;
            this.setOrgValue(orgValue);
        }

        public override object Clone()
        {
            return new SqlIdentifierExpr(this.value, this.getOrgValue());
        }

        public override string getOrgValue()
        {
            if ((this.orgValue != null) && (this.orgValue.Trim().Length != 0))
            {
                return this.orgValue;
            }
            return this.value;
        }

        public void setOrgValue(string orgValue)
        {
            this.orgValue = orgValue;
        }

        public void setValue(string val)
        {
            this.value = val;
        }
    }


   



}
