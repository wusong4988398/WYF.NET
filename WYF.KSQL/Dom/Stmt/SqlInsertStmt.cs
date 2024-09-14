using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlInsertStmt : SqlStmt
    {
        // Fields
        public SqlInsert insert;

        // Methods
        public SqlInsertStmt() : base(2)
        {
        }

        public SqlInsertStmt(SqlInsert insert) : base(2)
        {
            this.insert = insert;
        }

        public object clone()
        {
            SqlInsertStmt stmt = new SqlInsertStmt();
            if (this.insert != null)
            {
                stmt.insert = (SqlInsert)this.insert.Clone();
            }
            return stmt;
        }
    }


   



}
