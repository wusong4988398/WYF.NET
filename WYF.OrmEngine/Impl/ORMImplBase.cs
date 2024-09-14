using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.OrmEngine.Query;

namespace WYF.OrmEngine.Impl
{
    public abstract class ORMImplBase
    {
        protected readonly Dictionary<string, IDataEntityType> entityTypeCache;
        protected readonly ORMHint ormHint;

        protected readonly ORMOptimization optimization;
        protected ORMImplBase(Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, ORMOptimization optimization)
        {
            this.entityTypeCache = entityTypeCache;
            this.ormHint = ormHint;
            this.optimization = optimization;
        }
        public abstract DataSet QueryDataSet(string algoKey, string entityName, string selectFields, bool shouldSelectPK, QFilter[] filters, string groupBys, QFilter[] havings, string orderBys, int from, int length, IDistinctable distinctable);
        
        public abstract List<Object> Insert(List<DynamicObject> objs);

    }
}
