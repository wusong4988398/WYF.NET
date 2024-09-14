using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.OrmEngine.Query.Multi;

namespace WYF.OrmEngine.Query.OQL.G.expr
{
    public interface OQLable
    {
        PropertySegExpress ToExpress();
    }

}
