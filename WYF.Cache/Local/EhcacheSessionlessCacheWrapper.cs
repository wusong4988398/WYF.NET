using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace WYF.Cache.Local
{
    public class EhcacheSessionlessCacheWrapper : ILocalSessionCacheRegionManger
    {

        public FusionCache Cache { get;  set; }
        EhcacheSessionlessCache ehcache;
        string region;
        string type;

        public EhcacheSessionlessCacheWrapper(string region, string type, FusionCache cache, EhcacheSessionlessCache ehcache)
        {
            this.region = region;
            this.type = type;
            this.Cache = cache;
            this.ehcache = ehcache;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public ILocalMemoryCache CreateOrReplaceCache(string type, CacheConfigInfo cacheConfig)
        {
            throw new NotImplementedException();
        }

        public void Destory()
        {
            throw new NotImplementedException();
        }

        public object? Get(string key)
        {
            if (this.Cache == null)
                return null;
            return this.Cache.GetOrDefault<object>(key);

        }

        public IDictionary<string, object> Get(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public ILocalMemoryCache GetCache(string type)
        {
            throw new NotImplementedException();
        }

        public ILocalMemoryCache GetOrCreateCache(string type, CacheConfigInfo cacheConfig)
        {
            throw new NotImplementedException();
        }

        public ILocalMemoryCache GetOrCreateCache(string type)
        {
            throw new NotImplementedException();
        }

        public void Put(string key, object value)
        {
            this.Cache.Set(key, value);
        }

        public void Remove(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public void RemoveMapFields(string key, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public void RemoveType(string type)
        {
            throw new NotImplementedException();
        }
    }
}
