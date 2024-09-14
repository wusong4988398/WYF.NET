using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom.Stmt
{
    [Serializable]
    public class SqlSetLocalVariantStmt : SqlStmt
    {
        // Fields
        public SqlExpr value;
        public SqlVarRefExpr variant;

        // Methods
        public SqlSetLocalVariantStmt() : base(10)
        {
        }

        public SqlSetLocalVariantStmt(SqlVarRefExpr variant, SqlExpr value) : base(10)
        {
            this.variant = variant;
            this.value = value;
        }
    }


   



}
