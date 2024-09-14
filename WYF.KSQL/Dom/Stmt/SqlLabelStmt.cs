using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class SqlLabelStmt : SqlStmt
    {
        // Fields
        public bool isNullLable;
        public string name;

        // Methods
        public SqlLabelStmt() : base(100)
        {
        }
    }



}
