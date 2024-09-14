using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Schema;

namespace WYF.Algo.Sql.Tree
{
    public interface IUnresolved
    {
        Expr Resolve(ISchema schema);

    }
}
