using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using WYF.DbEngine.db;
using WYF.OrmEngine.Query;
using WYF.OrmEngine.Query.Multi;

namespace WYF.OrmEngine.Impl
{
    public class ORMImplStandard : ORMImplBase
    {
        public ORMImplStandard(Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, ORMOptimization optimization)
            : base(entityTypeCache, ormHint, optimization)
        {

        }

        public override List<object> Insert(List<DynamicObject> objs)
        {
            throw new NotImplementedException();
        }

        public override IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, bool shouldSelectPK, QFilter[] filters, string groupBys, QFilter[] havings, string orderBys, int from, int length, IDistinctable distinctable)
        {
            int top = from < 0 ? 0 : from;
            if (length < 0)
            {
                top = -1;
                length = int.MaxValue;
            }
            else
            {
                top = from + length;
            }
            MultiQuery mq = DoCreateMultiQuery(entityName, selectFields, shouldSelectPK, filters, groupBys, havings, orderBys, top, from, length, distinctable);
            IDataSet ds = mq.Query(algoKey);
            return ds;
        }




        private MultiQuery DoCreateMultiQuery(String entityName, String selectFields, bool shouldSelectPK, QFilter[] filters, String groupBys, QFilter[] havings, String orderBys, int top, int start, int length, IDistinctable distinctable)
        {
            IDataEntityType dt = ORMConfiguration.InnerGetDataEntityType(entityName, this.entityTypeCache);
            DBRoute dbRoute = new DBRoute(dt.DBRouteKey);
            DataEntityPropertyCollection fields = dt.Properties;
            MultiQuery mq = MultiQuery.Create(dbRoute, dt, selectFields, shouldSelectPK, filters, groupBys, havings, orderBys, top, start, length, this.entityTypeCache, this.ormHint, this.optimization, distinctable);
            return mq;
        }



    }
}
