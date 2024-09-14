using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlInListExpr : SqlExpr
    {
        // Fields
        public SqlExpr expr;
        public bool not;
        public ArrayList targetList;

        // Methods
        public SqlInListExpr() : base(14)
        {
            this.targetList = new ArrayList();
        }

        public SqlInListExpr(SqlExpr expr) : base(14)
        {
            this.targetList = new ArrayList();
            this.expr = expr;
        }

        public SqlInListExpr(SqlExpr expr, bool not) : base(14)
        {
            this.targetList = new ArrayList();
            this.expr = expr;
            this.not = not;
        }

        public override object Clone()
        {
            SqlInListExpr expr = new SqlInListExpr
            {
                not = this.not
            };
            if (this.expr != null)
            {
                expr.expr = (SqlExpr)this.expr.Clone();
            }
            if (this.targetList != null)
            {
                int num = 0;
                int count = this.targetList.Count;
                while (num < count)
                {
                    SqlExpr expr2 = (SqlExpr)((SqlExpr)this.targetList[num]).Clone();
                    expr.targetList.Add(expr2);
                    num++;
                }
            }
            return expr;
        }
    }





}
