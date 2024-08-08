using WYF.Bos.algo.sql.schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public interface IUnresolved
    {
        Expr Resolve(ISchema schema);
    }

}
