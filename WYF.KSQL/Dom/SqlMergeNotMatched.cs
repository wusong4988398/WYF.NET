using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    public class SqlMergeNotMatched
    {
        // Fields
        public List<SqlExpr> InsertColumns = new List<SqlExpr>();
        public List<SqlExpr> InsertValues = new List<SqlExpr>();
        public SqlExpr InsertWhere;
    }





}
