using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class SqlCursorLoopStmt : SqlStmt
    {
        // Fields
        public string curName;
        public IList fieldList;
        public IList intoList;
        public IList stmtList;

        // Methods
        public SqlCursorLoopStmt() : base(100)
        {
            this.stmtList = new ArrayList();
        }
    }


   



}
