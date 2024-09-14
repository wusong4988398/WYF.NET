using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlUpdateItem : AbstractUpdateItem
    {
        // Fields
        public SqlExpr expr;
        public string name;

        // Methods
        public SqlUpdateItem()
        {
        }

        public SqlUpdateItem(string name, SqlExpr expr)
        {
            this.name = name;
            this.expr = expr;
        }

        public override object Clone()
        {
            SqlUpdateItem item = new SqlUpdateItem
            {
                name = this.name
            };
            if (this.expr != null)
            {
                item.expr = (SqlExpr)this.expr.Clone();
            }
            return item;
        }
    }





}
