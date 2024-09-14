using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.SqlParser
{
    public interface IUnresolved
    {
        Expr Resolve(ISchema schema);

    }
}
