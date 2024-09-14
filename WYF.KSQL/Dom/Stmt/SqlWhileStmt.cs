using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlWhileStmt : SqlStmt
    {
        // Fields
        public SqlExpr condition;
        public IList stmtList;

        // Methods
        public SqlWhileStmt() : base(7)
        {
        }

        public SqlWhileStmt(SqlExpr condition) : base(7)
        {
            this.condition = condition;
        }
    }






}
