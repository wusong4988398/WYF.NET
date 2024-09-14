using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class SqlBreakStmt : SqlStmt
    {
        // Methods
        public SqlBreakStmt() : base(100)
        {
        }

        public override object Clone()
        {
            return new SqlBreakStmt();
        }
    }






}
