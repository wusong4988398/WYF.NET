using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.expr
{
    public abstract class Parser
    {
        public abstract Expr ParseExpr(string expr);

        public abstract Expr ParseSortList(string expr);
    }
}
