using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.Query
{
    public interface IDistinctable
    {
        bool Distinct(IDataEntityType entityType, Dictionary<string, bool> joinEntitySelectField);

    }
}
