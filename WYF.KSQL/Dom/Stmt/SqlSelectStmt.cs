using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlSelectStmt : SqlStmt
    {
        // Fields
        public SqlSelectBase select;

        // Methods
        public SqlSelectStmt() : base(0)
        {
        }

        public SqlSelectStmt(SqlSelectBase select) : base(0)
        {
            this.select = select;
        }

        public object clone()
        {
            SqlSelectStmt stmt = new SqlSelectStmt();
            if (this.select != null)
            {
                stmt.select = (SqlSelectBase)this.select.Clone();
            }
            return stmt;
        }
    }





}
