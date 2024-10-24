using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache.Local;
using WYF.Cache.redis;

namespace WYF.Cache
{
    public class CacheFactoryImpl : CacheFactory
    {
        
        private static ConcurrentDictionary<string, RedisSessionlessCache> distributeSessionlessCaches = new ConcurrentDictionary<string, RedisSessionlessCache>();

        private static ConcurrentDictionary<string, ILocalMemoryCache> localSessionlessCaches = new ConcurrentDictionary<string, ILocalMemoryCache>();
        private IDistributeSessionlessCache distributeSessionlessCache;


        public CacheFactoryImpl()
        {
            this.distributeSessionlessCache = CreateDistributeSessionlessCache();
        }

        private IDistributeSessionlessCache? CreateDistributeSessionlessCache()
        {
            RedisSessionlessCache cache = new RedisSessionlessCache(null);
            return (IDistributeSessionlessCache)cache;
        }

        public override ILocalMemoryCache GetOrCreateLocalMemoryCache(string region, string type)
        {
            ILocalMemoryCache cache = GetLocalSessionlessCache(region);
            return ((ILocalSessionCacheRegionManger)cache).GetOrCreateCache(type);
        }
        public override ILocalMemoryCache GetOrCreateLocalMemoryCache(string region, string type,
                             CacheConfigInfo cacheConfig)
        {
            ILocalMemoryCache cache = GetLocalSessionlessCache(region);
            return ((ILocalSessionCacheRegionManger)cache).GetOrCreateCache(type, cacheConfig);
        }

        private ILocalMemoryCache GetLocalSessionlessCache(string region)
        {
            if (region == null)
                throw new Exception("local memory cache's region cannot be null");
            if (!localSessionlessCaches.TryGetValue(region, out ILocalMemoryCache cache))
            {
                lock (localSessionlessCaches)
                {
                    if (!localSessionlessCaches.TryGetValue(region, out cache))
                    {
                        cache = CreateLocalSessionlessCache(region);
                        localSessionlessCaches[region] = cache;
                    }
                }
            }
            return cache;
        }

        private ILocalMemoryCache CreateLocalSessionlessCache(string region)
        {
            EhcacheSessionlessCache cache = new EhcacheSessionlessCache(region);
            return (ILocalMemoryCache)cache;
        }

        public override IDistributeSessionlessCache GetDistributeSessionlessCache()
        {
            return this.distributeSessionlessCache;
        }

        public override IDistributeSessionlessCache GetDistributeSessionlessCache(string region)
        {
            if (string.IsNullOrEmpty(region)) return this.distributeSessionlessCache;

            RedisSessionlessCache cache = distributeSessionlessCaches.GetValueOrDefault(region, null);
            if (cache==null)
            {
                lock (distributeSessionlessCaches)
                {
                    cache = distributeSessionlessCaches.GetValueOrDefault(region, null);
                    if (cache==null)
                    {
                        cache = new RedisSessionlessCache(region);
                        distributeSessionlessCaches[region]=cache;
                    }
                }
            }
            return cache;


        }
    }
}
