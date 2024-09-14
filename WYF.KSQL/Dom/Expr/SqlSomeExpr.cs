using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlSomeExpr : SqlExpr
    {
        // Fields
        public SqlSelectBase subQuery;

        // Methods
        public SqlSomeExpr() : base(0x13)
        {
        }

        public SqlSomeExpr(SqlSelectBase subQuery) : base(0x13)
        {
            this.subQuery = subQuery;
            this.setExprWord("SOME");
        }

        public SqlSomeExpr(string someWord, SqlSelectBase subQuery) : base(0x13)
        {
            this.subQuery = subQuery;
            this.setExprWord(someWord);
        }

        public override object Clone()
        {
            SqlSomeExpr expr = new SqlSomeExpr();
            if (this.subQuery != null)
            {
                expr.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            return expr;
        }
    }





}
