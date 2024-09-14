using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlNullExpr : SqlExpr
    {
        // Fields
        public static readonly SqlNullExpr instance = new SqlNullExpr();

        // Methods
        public SqlNullExpr() : base(20)
        {
            this.setExprWord("NULL");
        }

        public override object Clone()
        {
            return instance;
        }

        public override void output(StringBuilder buff)
        {
            string str = this.getExprWord();
            if ((str == null) || (str.Length == 0))
            {
                str = "NULL";
            }
            buff.Append(str);
        }

        public override string toString()
        {
            string str = this.getExprWord();
            if ((str != null) && (str.Length != 0))
            {
                return str;
            }
            return "NULL";
        }
    }


 


}
