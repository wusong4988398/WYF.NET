using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class SqlContinueStmt : SqlStmt
    {
        // Methods
        public SqlContinueStmt() : base(100)
        {
        }

        public override object Clone()
        {
            return new SqlContinueStmt();
        }
    }






}
