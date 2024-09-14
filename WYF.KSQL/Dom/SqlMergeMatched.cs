using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    public class SqlMergeMatched
    {
        // Fields
        public SqlExpr DeleteWhere;
        public List<SqlExpr> SetClauses = new List<SqlExpr>();
        public SqlExpr UpdateWhere;
    }





}
