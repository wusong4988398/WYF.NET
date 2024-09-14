using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.OQL.G.expr
{
    public enum SegmentType
    {
        select_fields,
        where_filters,
        orderbys,
        groupbys
    }
}
