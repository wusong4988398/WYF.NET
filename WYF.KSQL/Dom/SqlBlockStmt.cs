using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;

namespace WYF.KSQL.Dom
{
    public class SqlBlockStmt : SqlStmt
    {
        // Fields
        public IList declItemList;
        public IList stmtList;

        // Methods
        public SqlBlockStmt() : base(100)
        {
            this.declItemList = new ArrayList();
            this.stmtList = new ArrayList();
        }

        // Nested Types
        public class DeclCurItem : SqlBlockStmt.DeclItem
        {
            // Fields
            public SqlSelectBase select;
        }

        public abstract class DeclItem
        {
            // Fields
            public string name;

            // Methods
            protected DeclItem()
            {
            }
        }

        public class DeclVarItem : SqlBlockStmt.DeclItem
        {
            // Fields
            public string dataType;
            public SqlExpr defaultValueExpr;
            public int length;
            public int precision;
            public int scale;
        }
    }



  








}
