using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlMergeStmt : SqlStmt
    {
        // Fields
        public SqlMerge Merge;

        // Methods
        public SqlMergeStmt(SqlMerge merge) : base(0x31)
        {
            this.Merge = merge;
        }
    }


   



}
