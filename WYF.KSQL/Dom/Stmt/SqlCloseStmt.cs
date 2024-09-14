using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class SqlCloseStmt : SqlStmt
    {
        // Fields
        public string curName;

        // Methods
        public SqlCloseStmt() : base(100)
        {
        }

        public override object Clone()
        {
            return new SqlCloseStmt();
        }
    }


   



}
