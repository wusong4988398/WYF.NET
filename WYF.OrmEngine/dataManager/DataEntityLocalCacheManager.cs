using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text;
using WYF.Cache;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.OrmEngine.Query;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 用于管理本地内存缓存的类，主要用于缓存数据实体（DataEntity）及其相关过滤器的结果
    /// </summary>
    public class DataEntityLocalCacheManager
    {
        private ILocalMemoryCache rootCache;
        private IDictionary<object, object> subDataEntityCache;
        private IDataEntityType dt;
        private string rootType;
        private string rootFilterType;

        public DataEntityLocalCacheManager(string tableName)
        {
            this.rootType = tableName.ToLower();
            this.rootFilterType = this.rootType + ".filter";
            var info = new CacheConfigInfo
            {
                Timeout = 3600,
                MaxMemSize = 300
            };
            this.rootCache = CacheFactory.GetCommonCacheFactory().GetOrCreateLocalMemoryCache(
                "ClusterName" + "." + RequestContext.Current.AccountId, "bd", info);
        }

        public object[] GetCachePks(QFilter[] filters)
        {
            var key = GetFilterKey(filters);
            return GetFilterCache().GetOrDefault(key);
        }

        public void PutCachePks(QFilter[] filters, object[] pks)
        {
            var key = GetFilterKey(filters);
            GetFilterCache()[key] = pks;
        }

        private string GetFilterKey(QFilter[] filters)
        {
            if (filters == null || filters.Length == 0)
                return "null";

            if (filters.Length == 1)
                return filters[0].ToString();

            var sb = new StringBuilder(filters[0].ToString());
            for (int i = 1; i < filters.Length; i++)
                sb.Append(',').Append(filters[i]);

            return sb.ToString();
        }

        private IDictionary<string, object[]> GetFilterCache()
        {
            var filterCache = (IDictionary<string, object[]>)this.rootCache.Get(this.rootFilterType);
            if (filterCache == null)
            {
                filterCache = new ConcurrentDictionary<string, object[]>();
                this.rootCache.Put(this.rootFilterType, filterCache);
            }
            return filterCache;
        }

        private IDictionary<string, IDictionary<object, object>> GetDataEntityCache()
        {
            var dataEntityCache = (IDictionary<string, IDictionary<object, object>>)this.rootCache.Get(this.rootType);
            if (dataEntityCache == null)
            {
                dataEntityCache = new ConcurrentDictionary<string, IDictionary<object, object>>();
                this.rootCache.Put(this.rootType, dataEntityCache);
            }
            return dataEntityCache;
        }

        public DataEntityLocalCacheManager(IDataEntityType dt) : this(dt.Alias?.ToLower() ?? string.Empty)
        {
            if (dt.PrimaryKey == null)
                throw new ArgumentException("实体缺乏主键，无法缓存处理。");

            this.dt = dt;
            this.subDataEntityCache = GetCache(GetSubType());
        }

        private IDictionary<object, object> GetCache(string subType)
        {
            var dataEntityCache = GetDataEntityCache();
            var retCache = dataEntityCache.GetOrDefault(subType);
            if (retCache == null)
            {
                retCache = new ConcurrentDictionary<object, object>();
                dataEntityCache[subType] = retCache;
            }
            return retCache;
        }

        private IDictionary<object, object> Get(string[] pks, List<string> notFindpks)
        {
            var cacheMap = new Dictionary<object, object>();
            if (pks.Length > 0)
            {
                var iSimpleProperty = this.dt.PrimaryKey;
                foreach (var pk in pks)
                {
                    var obj = this.subDataEntityCache.GetOrDefault(pk);
                    if (obj != null)
                    {
                        cacheMap.Add(iSimpleProperty.GetValueFast(obj), obj);
                    }
                    else
                    {
                        notFindpks.Add(pk);
                    }
                }
            }
            return cacheMap;
        }

        public void Put(object[] dataEntities)
        {
            var iSimpleProperty = this.dt.PrimaryKey;
            foreach (var obj in dataEntities)
            {
                this.subDataEntityCache[GetKey(iSimpleProperty.GetValueFast(obj))] = obj;
            }
        }

        public void Put(IDictionary<object, object> dataEntities)
        {

            foreach (var kvp in dataEntities)
            {
                subDataEntityCache[kvp.Key] = kvp.Value;
            }

        }

        public void RemoveByDt()
        {
            this.rootCache.Remove(new[] { this.rootType });
            RemoveByFilterDt();
        }

        public void RemoveByFilterDt()
        {
            this.rootCache.Remove(new[] { this.rootFilterType });
        }

        private string GetSubType()
        {
            string subType;
            if (this.dt is DynamicObjectType dynamicType)
            {
                subType = dynamicType.ExtendName;
            }
            else
            {
                subType = this.dt.Name;
            }
            return subType;
        }

        private string GetKey(object id)
        {
            return id.ToString();
        }

        public IDictionary<object, object> Get(object[] pkArray, List<string> notFindpks)
        {
            var pks = new string[pkArray.Length];
            for (int i = 0; i < pkArray.Length; i++)
                pks[i] = pkArray[i].ToString();
            return Get(pks, notFindpks);
        }

        public IDataEntityType GetDataEntityType()
        {
            return this.dt;
        }
    }
}