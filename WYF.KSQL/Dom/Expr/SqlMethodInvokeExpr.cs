using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlMethodInvokeExpr : SqlExpr
    {
        // Fields
        public string methodName;
        public SqlExpr owner;
        public ArrayList parameters;

        // Methods
        public SqlMethodInvokeExpr() : base(10)
        {
            this.parameters = new ArrayList();
        }

        public SqlMethodInvokeExpr(string methodName) : base(10)
        {
            this.parameters = new ArrayList();
            this.methodName = methodName;
            this.setExprWord(methodName);
        }

        public SqlMethodInvokeExpr(SqlExpr owner, string methodName) : base(10)
        {
            this.parameters = new ArrayList();
            this.owner = owner;
            this.methodName = methodName;
            this.setExprWord(methodName);
        }

        public override object Clone()
        {
            SqlMethodInvokeExpr expr = new SqlMethodInvokeExpr(this.methodName);
            if (this.owner != null)
            {
                expr.owner = (SqlExpr)this.owner.Clone();
            }
            if (this.parameters != null)
            {
                int num = 0;
                int count = this.parameters.Count;
                while (num < count)
                {
                    SqlExpr expr2 = (SqlExpr)((SqlExpr)this.parameters[num]).Clone();
                    expr.parameters.Add(expr2);
                    num++;
                }
            }
            expr.setExprWord(this.getExprWord());
            return expr;
        }
    }


  



}
