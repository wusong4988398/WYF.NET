using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlShowTablesStmt : SqlStmt
    {
        // Methods
        public SqlShowTablesStmt() : base(0x2f)
        {
        }

        public object clone()
        {
            return new SqlShowTablesStmt();
        }
    }


   



}
