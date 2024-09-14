using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlTrancateTableStmt : SqlStmt
    {
        // Fields
        public string tableName;

        // Methods
        public SqlTrancateTableStmt() : base(12)
        {
        }

        public SqlTrancateTableStmt(string tableName) : base(12)
        {
            this.tableName = tableName;
        }
    }


  


}
