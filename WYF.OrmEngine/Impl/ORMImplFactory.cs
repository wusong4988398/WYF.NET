using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.Impl
{
    public class ORMImplFactory
    {
        public static ORMImplBase CreateORMImpl(Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, ORMOptimization optimization)
        {
            return new ORMImplStandard(entityTypeCache, ormHint, optimization);

        }
    }
}
