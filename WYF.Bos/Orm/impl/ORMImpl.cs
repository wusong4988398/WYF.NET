using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Orm.query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public class ORMImpl:ORM
    {
        private ORMImplBase impl;
        private ORMHint ormHint;
        private ORMOptimization optimization;
        private ORMCostStat stat;
        private ORMEntityTypeCacheMap entityTypeCache;
        private static ThreadLocal<Dictionary<string, IDataEntityType>> tlEntityTypeCache;
        private static ThreadLocal<long> tlEntityTypeCacheCreateTime;
        private static ThreadLocal<string> tlEntityTypeCacheCreateThread;

        static ORMImpl()
        {
            ORMImpl.tlEntityTypeCacheCreateTime = new ThreadLocal<long>() { Value = (long)(Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000000000.0)) };
            ORMImpl.tlEntityTypeCacheCreateThread = new ThreadLocal<string>() { Value = Thread.CurrentThread.Name };
            tlEntityTypeCache = new ThreadLocal<Dictionary<string, IDataEntityType>>(() =>
            {
                ORMImpl.tlEntityTypeCacheCreateTime.Value = (long)(Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000000000.0));
      

                return new Dictionary<string, IDataEntityType>(20);
            });


        }
        public ORMImpl()
        {
            this.ormHint = new ORMHint();
            this.optimization = new ORMOptimization();
            this.entityTypeCache = new ORMEntityTypeCacheMap(ORMImpl.tlEntityTypeCache.Value, ORMImpl.tlEntityTypeCacheCreateTime.Value, ORMImpl.tlEntityTypeCacheCreateThread.Value);
            this.stat = ORMCostStat.Get();
            this.impl = ORMImplFactory.CreateORMImpl(this.entityTypeCache, this.ormHint, this.optimization);
        }

        public DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int top, IDistinctable distinctable)
        {
            return this.QueryDataSet(algoKey, entityName, selectFields, filters, orderBys, 0, top, distinctable);
        }

        public DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, IDistinctable distinctable)
        {
            this.stat.BeginTrace();

            return this.impl.QueryDataSet(algoKey, entityName, selectFields, false, filters, null, null, orderBys, from, length, distinctable);

        }
        public DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, Func<IDataEntityType, Dictionary<string, bool>, bool> distinctable)
        {
            throw new NotImplementedException();
        }

    }
}
