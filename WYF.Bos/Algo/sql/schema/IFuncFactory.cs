using WYF.Bos.algo.sql.tree;
using WYF.Bos.algo.sql.tree.func;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.schema
{
    public interface IFuncFactory
    {
        IFuncDef LookupFunc(string name, Expr[] children);

    }
}
