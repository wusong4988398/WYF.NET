using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlAlterViewStmt : SqlStmt
    {
        // Fields
        public string name;
        public SqlSelectBase select;

        // Methods
        public SqlAlterViewStmt() : base(0x23)
        {
        }

        public SqlAlterViewStmt(string name) : base(0x23)
        {
            this.name = name;
        }

        public SqlAlterViewStmt(SqlSelectBase select, string name) : base(0x23)
        {
            this.select = select;
            this.name = name;
        }

        public override object Clone()
        {
            SqlAlterViewStmt stmt = new SqlAlterViewStmt(this.name);
            if (this.select != null)
            {
                stmt.select = (SqlSelectBase)this.select.Clone();
            }
            return stmt;
        }
    }




}
