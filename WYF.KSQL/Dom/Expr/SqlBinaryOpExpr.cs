using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlBinaryOpExpr : SqlExpr
    {
        // Fields
        public SqlExpr left;
        public int Operator;
        public SqlExpr right;

        // Methods
        public SqlBinaryOpExpr() : base(0)
        {
        }

        public SqlBinaryOpExpr(SqlExpr left, int _operator, SqlExpr right) : base(0)
        {
            this.left = left;
            this.right = right;
            this.Operator = _operator;
        }

        public override object Clone()
        {
            SqlBinaryOpExpr expr = new SqlBinaryOpExpr();
            if (this.left != null)
            {
                expr.left = (SqlExpr)this.left.Clone();
            }
            if (this.right != null)
            {
                expr.right = (SqlExpr)this.right.Clone();
            }
            expr.Operator = this.Operator;
            return expr;
        }

        public override string getExprWord()
        {
            string str = base.getExprWord();
            if ((str != null) && (str.Length != 0))
            {
                return str;
            }
            int @operator = this.Operator;
            if (@operator <= 13)
            {
                switch (@operator)
                {
                    case 7:
                        return "AND";

                    case 8:
                        return "OR";

                    case 13:
                        return "IS";

                    case 1:
                        return "AS";
                }
                return str;
            }
            if (@operator != 0x12)
            {
                switch (@operator)
                {
                    case 40:
                        return "NOT LIKE";

                    case 0x29:
                        return "IS NOT";

                    case 0x2a:
                        return str;

                    case 0x2b:
                        return "ESCAPE";

                    case 0x1b:
                        return "UNION";
                }
                return str;
            }
            return "LIKE";
        }
    }






}
