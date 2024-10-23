using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.dataset;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.DataEntity.Metadata;
using WYF.OrmEngine.Query;
using WYF.Cache;
using WYF.DataEntity.Serialization;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 类是一个用于管理数据实体缓存的类。它提供了多种方法来处理数据实体的缓存操作，包括获取、存储和删除缓存中的数据实体。
    /// </summary>
    public class DataEntityCacheManager : IDataEntityCacheManager
    {
        //private static readonly IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache(null, new DistributeCacheHAPolicy());

        private static readonly IDistributeSessionlessCache cache = CacheFactoryBuilder.NewBuilder().Build().GetDistributeSessionlessCache();

        //private static readonly ILogger logger = LogFactory.GetLogger(typeof(DataEntityCacheManager));
        private IDataEntityType dt;
        private string regionKey;
        private string rootType;
        private string numberRegionKey;

        public DataEntityCacheManager(string tableName)
        {
            this.rootType = tableName.ToLower();
        }

        public DataEntityCacheManager(IDataEntityType dt)
        {
            if (dt == null)
            {
                throw new ArgumentException("dt");
            }
            else if (dt.PrimaryKey == null)
            {
                throw new ArgumentException("实体缺乏主键，无法缓存处理。");
            }
            else
            {
                this.dt = dt;
                this.rootType = dt.Alias?.ToLower() ?? string.Empty;
                this.regionKey = GetSubType();
                this.numberRegionKey = this.regionKey + "_number";
            }
        }

        public object[] GetCachePks(QFilter[] filters)
        {
            var localCacheManager = new DataEntityLocalCacheManager(dt);
            return localCacheManager.GetCachePks(filters);
        }

        public void PutCachePks(QFilter[] filters, object[] pks)
        {
            var localCacheManager = new DataEntityLocalCacheManager(dt);
            localCacheManager.PutCachePks(filters, pks);
        }

        private IDictionary<object, object> Get(string[] pks)
        {
            IDictionary<object, object> cacheMap;
            if (pks.Length > 0)
            {
                var localCacheManager = new DataEntityLocalCacheManager(dt);
                var notFindpks = new List<string>();
                cacheMap = localCacheManager.Get(pks.Cast<object>().ToArray(), notFindpks);

                if (notFindpks.Count > 0)
                {
                    var redisCacheMap = new Dictionary<object, object>();
                    var strs = cache.Get(regionKey, notFindpks.ToArray());

                    var pkProp = dt.PrimaryKey;
                    foreach (var strObj in strs)
                    {
                        if (!string.IsNullOrWhiteSpace(strObj))
                        {
                            try
                            {
                                var mapObject = SerializationUtils.FromJsonString<IDictionary<string, object>>(strObj);
                                var obj = DataEntitySerializer.ConvertMapToDataEntity(dt, mapObject);
                                redisCacheMap[pkProp.GetValueFast(obj)] = obj;
                            }
                            catch (Exception e)
                            {
                               // throw e;
                                //logger.Error($"读取缓存失败 {dt.Name} {regionKey}", e);
                            }
                        }
                    }
                    cacheMap = cacheMap.Concat(redisCacheMap).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    localCacheManager.Put(redisCacheMap.Values.ToArray());
                }
            }
            else
            {
                cacheMap = new Dictionary<object, object>();
            }
            return cacheMap;
        }

        public void Put(object[] dataEntities)
        {
            PutType();
            var cacheMap = new Dictionary<string, string>(dataEntities.Length);
            var pkProp = dt.PrimaryKey;
            foreach (var obj in dataEntities)
            {
                cacheMap[GetKey(pkProp.GetValueFast(obj))] = DataEntitySerializer.SerializerToString(obj);
            }

            try
            {
                if (cacheMap.Count > 0)
                {
                    cache.Put(regionKey, cacheMap);
                }
                var lCacheManager = new DataEntityLocalCacheManager(dt);
                lCacheManager.Put(dataEntities);
            }
            catch (Exception e)
            {
               throw new Exception(e.Message  + string.Join(", ", cacheMap), e);
            }
        }

        //public void PutNotExistPKs(object[] oids, object[] dataEntities)
        //{
        //    var localCacheManager = new DataEntityLocalCacheManager(dt);
        //    localCacheManager.PutNotExistPKs(oids, dataEntities);
        //}

        //public void PutNotExistPKs(object[] notExistPKs)
        //{
        //    var localCacheManager = new DataEntityLocalCacheManager(dt);
        //    localCacheManager.PutNotExistPKs(notExistPKs);
        //}

        //public bool IsNotExistPK(object pk)
        //{
        //    var localCacheManager = new DataEntityLocalCacheManager(dt);
        //    return localCacheManager.IsNotExistPK(pk);
        //}

        public void RemoveByPrimaryKey(params object[] pks)
        {
            if (pks != null)
            {
                var listPk = new List<string>();
                foreach (var pk in pks)
                {
                    if (pk != null)
                    {
                        listPk.Add(pk.ToString());
                    }
                }
                if (listPk.Count > 0)
                {
                    var keys = listPk.ToArray();
                    var subTypes = GetTypes();
                    foreach (var subType in subTypes)
                    {
                        cache.Remove(subType, keys);
                    }
                }
                var lCacheManager = new DataEntityLocalCacheManager(rootType);
                lCacheManager.RemoveByDt();
            }
        }

        public void RemoveByDt()
        {
            var subTypes = GetTypes();
            cache.Remove(subTypes.ToArray());
            if (!string.IsNullOrWhiteSpace(numberRegionKey))
            {
                cache.Remove(numberRegionKey);
            }
            var lCacheManager = new DataEntityLocalCacheManager(rootType);
            lCacheManager.RemoveByDt();
        }

        public void RemoveByFilterDt()
        {
            var lCacheManager = new DataEntityLocalCacheManager(rootType);
            lCacheManager.RemoveByFilterDt();
        }

        private void PutType()
        {
            var subtype = new Dictionary<string, string>
            {
                { GetSubType(), "1" }
            };
            cache.Put(GetRootType(), subtype);
        }

        private ICollection<string> GetTypes()
        {
            var subTypes = cache.GetAll(GetRootType());
            return subTypes.Keys;
        }

        private string GetRootType()
        {
            var context = RequestContext.Current;
            return $"{context.AccountId}.{rootType.ToLower()}";
        }

        private string GetSubType()
        {
            string region;
            if (dt is DynamicObjectType dynamicType)
            {
                region = dynamicType.ExtendName;
            }
            else
            {
                region = dt.Name;
            }
            var context = RequestContext.Current;
            return $"{context.AccountId}.{region}";
        }

        private string GetKey(object id)
        {
            return id.ToString();
        }

        public IDictionary<object, object> Get(object[] pkArray)
        {
            var pks = pkArray.Select(pk => pk.ToString()).ToArray();
            return Get(pks);
        }

        public IDataEntityType GetDataEntityType()
        {
            return dt;
        }

        public IDictionary<object, object> GetByNumbers(params string[] numbers)
        {
            var pkNumberMap = new Dictionary<string, string>();
            foreach (var num in numbers)
            {
                if (!string.IsNullOrWhiteSpace(num))
                {
                    var pk = cache.Get(numberRegionKey, num);
                    if (!string.IsNullOrWhiteSpace(pk))
                    {
                        pkNumberMap[pk] = num;
                    }
                }
            }
            var pkObjs = Get(pkNumberMap.Keys.ToArray());
            var map = new Dictionary<object, object>(pkNumberMap.Count);
            foreach (var entry in pkObjs)
            {
                var number = pkNumberMap[entry.Key.ToString()];
                map[number] = entry.Value;
            }
            return map;
        }

        public void PutByNumbers(string numberPropKey, object[] dataEntities)
        {
            var cacheMap = new Dictionary<string, string>(dataEntities.Length);
            var pks = new string[dataEntities.Length];
            var pkProp = dt.PrimaryKey;
            var numberProp = dt.Properties[numberPropKey];
            for (int i = 0; i < dataEntities.Length; i++)
            {
                var obj = dataEntities[i];
                var pk = GetKey(pkProp.GetValueFast(obj));
                pks[i] = pk;
                cacheMap[GetKey(numberProp.GetValueFast(obj))] = pk;
            }

            try
            {
                if (cacheMap.Count > 0)
                {
                    cache.Put(numberRegionKey, cacheMap);
                }
                Put(dataEntities);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message  + string.Join(", ", cacheMap), e);
            }
        }

        private int GetDefaultTimeout()
        {
            var s = Environment.GetEnvironmentVariable("redis.defaulttimeout.dataentity");
            if (s != null)
            {
                return int.Parse(s.Trim());
            }
            return 100;
        
        }
    }
}

