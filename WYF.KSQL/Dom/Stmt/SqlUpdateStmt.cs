using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlUpdateStmt : SqlStmt
    {
        // Fields
        public SqlUpdate update;

        // Methods
        public SqlUpdateStmt(SqlUpdate update) : base(3)
        {
            this.update = update;
        }
    }






}
