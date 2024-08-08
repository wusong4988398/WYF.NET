using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public class ORMImplFactory
    {
        public static ORMImplBase CreateORMImpl(Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, ORMOptimization optimization)
        {
            return new ORMImplStandard(entityTypeCache, ormHint, optimization);

        }
    }
}
