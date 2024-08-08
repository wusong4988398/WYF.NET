using WYF.Bos.Orm.query.multi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.oql.g.expr
{
    public interface  OQLable
    {
        PropertySegExpress ToExpress();

    }
}
