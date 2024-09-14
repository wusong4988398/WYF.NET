using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlEmptyExpr : SqlExpr
    {
        // Fields
        public static readonly SqlEmptyExpr instance = new SqlEmptyExpr();

        // Methods
        public SqlEmptyExpr() : base(0x1d)
        {
            this.setExprWord("EMPTY");
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
                str = "EMPTY";
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
            return "EMPTY";
        }
    }






}
