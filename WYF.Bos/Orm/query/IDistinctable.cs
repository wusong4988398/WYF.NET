using WYF.Bos.DataEntity.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    public interface IDistinctable
    {
        bool Distinct(IDataEntityType entityType, Dictionary<string, bool> joinEntitySelectField);

    }
}
