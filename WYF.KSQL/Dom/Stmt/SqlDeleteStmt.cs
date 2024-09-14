using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlDeleteStmt : SqlStmt
    {
        // Fields
        public SqlDelete delete;

        // Methods
        public SqlDeleteStmt(SqlDelete delete) : base(1)
        {
            this.delete = delete;
        }
    }


  



}
