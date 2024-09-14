using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlShowColumnsStmt : SqlStmt
    {
        // Fields
        public string tableName;

        // Methods
        public SqlShowColumnsStmt() : base(0x2e)
        {
        }

        public SqlShowColumnsStmt(string tableName) : base(0x2e)
        {
            this.tableName = tableName;
        }
    }


  



}
