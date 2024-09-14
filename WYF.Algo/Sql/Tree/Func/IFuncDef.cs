using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.Func
{
    public interface IFuncDef
    {
        string Name { get; }
        Expr CreateExpr(Expr[] exprArr);

    }
}
