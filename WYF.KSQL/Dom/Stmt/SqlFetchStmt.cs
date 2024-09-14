using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Stmt
{
    public class SqlFetchStmt : SqlStmt
    {
        // Fields
        public string curName;
        public IList fieldList;
        public IList intoList;

        // Methods
        public SqlFetchStmt() : base(100)
        {
        }
    }


  



}
