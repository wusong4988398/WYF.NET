using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class CallStmt : SqlStmt
    {
        // Fields
        public IList paramList;
        public string procName;
        public SqlExpr returnExpr;

        // Methods
        public CallStmt() : base(0x30)
        {
            this.paramList = new ArrayList();
        }
    }


   



}
