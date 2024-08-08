using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.func
{
    public interface IFuncDef
    {
        public string Name { get; }

        public Expr CreateExpr(Expr[] children);
    }
}
