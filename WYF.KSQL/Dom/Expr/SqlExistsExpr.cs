using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlExistsExpr : SqlExpr
    {
        // Fields
        public bool not;
        public SqlSelectBase subQuery;

        // Methods
        public SqlExistsExpr() : base(15)
        {
            this.setExprWord("EXISTS");
        }

        public SqlExistsExpr(SqlSelectBase subQuery) : base(15)
        {
            this.subQuery = subQuery;
            this.setExprWord("EXISTS");
        }

        public SqlExistsExpr(SqlSelectBase subQuery, bool not) : base(15)
        {
            this.subQuery = subQuery;
            this.not = not;
            if (this.not)
            {
                this.setExprWord("NOT EXISTS");
            }
            else
            {
                this.setExprWord("EXISTS");
            }
        }

        public override object Clone()
        {
            SqlExistsExpr expr = new SqlExistsExpr
            {
                not = this.not
            };
            if (this.subQuery != null)
            {
                expr.subQuery = (SqlSelectBase)this.subQuery.Clone();
            }
            expr.setExprWord(this.getExprWord());
            return expr;
        }
    }





}
