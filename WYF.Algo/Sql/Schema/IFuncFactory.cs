using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.Func;
using WYF.Algo.Sql.Tree;

namespace WYF.Algo.Sql.Schema
{
    public interface IFuncFactory
    {
        IFuncDef LookupFunc(string str, Expr[] exprArr);


    }
}
