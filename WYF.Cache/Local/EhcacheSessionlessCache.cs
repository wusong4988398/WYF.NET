using CacheManager.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.Cache.Local
{
    public class EhcacheSessionlessCache : ILocalSessionCacheRegionManger
    {
        private static IDictionary<string, CacheProperites> configMap;
        private static  char REGIONTYPESPILTCHAR = '-';
        private string _region;
        private ConcurrentDictionary<string, EhcacheSessionlessCacheWrapper> cacheMap;
        private static string limitType;
        static EhcacheSessionlessCache()
        {
            configMap=new Dictionary<string, CacheProperites>();
            InitConfig();
        }

        private static void InitConfig()
        {
            //List<RegionDef> regionList = CacheConfig.get().getRegions();
            configMap["default-default"] = new CacheProperites { Name = "default", Timeout = 3600, MaxItemSize = 100000, MaxHeapSize = 100 };
        }

        public EhcacheSessionlessCache(string region)
        {
            this._region = "";
            this.cacheMap = new ConcurrentDictionary<string, EhcacheSessionlessCacheWrapper>();
            if (region != null)
            {
                this._region = region;
            }
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

        public object Get(string key)
        {
            return this.GetOrCreateCache(null).Get(key);
        }

        public IDictionary<string, object> Get(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public ILocalMemoryCache GetCache(string type)
        {
           return GetCache(type,false);
        }

        public ILocalMemoryCache GetOrCreateCache(string type, CacheConfigInfo cacheConfig)
        {
            ILocalMemoryCache cache = this.GetCache(type, false);
            if (cache==null)
            {
                lock (this)
                {
                    cache = this.GetCache(type, false);
                    if (cache==null)
                    {
                        string region = this.MakeInnerRegion(type);
                        CacheProperites cp = new CacheProperites();
                        cp.Name = type;
                        cp.Timeout = cacheConfig.Timeout;
                        cp.MaxHeapSize = cacheConfig.MaxMemSize;
                        cp.MaxItemSize = cacheConfig.MaxItemSize;
                        cp.IsTimeToLive = cacheConfig.IsTimeToLive;
                        EhcacheSessionlessCache.configMap[region] = cp;
                        cache = this.GetCache(type, true);
                    }
                }
            }
            return cache;
        }

        public ILocalMemoryCache GetOrCreateCache(string type)
        {
            ILocalMemoryCache cache = this.GetCache(type, true);
            return cache;

        }

        private EhcacheSessionlessCacheWrapper GetCache(string type,  bool createIfNotExists)
        {
            string region = this.MakeInnerRegion(type);
            EhcacheSessionlessCacheWrapper cacheWrapper = this.cacheMap[region];
            if ((cacheWrapper == null || cacheWrapper.Cache == null) && createIfNotExists)
            {
                lock (cacheMap) {
                    cacheWrapper = this.cacheMap[region];
                    if (cacheWrapper==null|| cacheWrapper.Cache==null)
                    {
                        CacheProperites cp = EhcacheSessionlessCache.configMap[region];
                        if (cp==null)
                        {
                            cp = EhcacheSessionlessCache.configMap[this._region];
                        }
                        if (cp==null)
                        {
                            throw new Exception("not config cache region of " + this._region + " , type of " + type);
                        }
                        if (cp.Timeout<=0)
                        {
                            throw new Exception(" cache config error , region of " + this._region + " , type of " + type + ", config:" + cp);
                        }
                        FusionCache cache = this.NewCache(region, cp.Timeout, cp.MaxHeapSize, cp.MaxItemSize, cp.IsTimeToLive);
                        if (cacheWrapper != null)
                        {
                            cacheWrapper.Cache = cache;
                
                        }
                        else
                        {
                            cacheWrapper = new EhcacheSessionlessCacheWrapper(this._region, type, cache, this);
                            this.cacheMap[region] = cacheWrapper;

                        }
                    }


                }
            }
            return cacheWrapper;
        }

        private FusionCache NewCache(string region, int timeout, int maxHeapSize, int maxItemSize, bool isTimeToLive)
        {

            //ICacheManager<object> manager=CacheManager.Core.CacheFactory.Build(region, settings =>
            //{
            //    settings.WithUpdateMode(CacheUpdateMode.Up)
            //    .WithMicrosoftMemoryCacheHandle()
            //    .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));

            //});

            var cache = new FusionCache(new FusionCacheOptions()
            {
                DefaultEntryOptions = new FusionCacheEntryOptions
                {
                    Duration = TimeSpan.FromMinutes(2),
                },
                CacheName = region,

            });
           

            //IMemoryCache cache = new MemoryCache(new MemoryCacheOptions { });


            return cache;

            
        }

        private  string MakeInnerRegion(string type)
        {
            if (type == null || type.Trim().Length == 0)
            {
                type = "default";
            }
            return this._region + '-' + type;
        }
        public void Put(string key, object value)
        {
            this.GetOrCreateCache(null).Put(key, value);
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


        private  class CacheProperites
        {
            public int Timeout {  get; set; }
            public int MaxHeapSize {  get; set; }
            public int MaxItemSize {  get; set; }
            public string Name {  get; set; }
            public bool IsTimeToLive {  get; set; }

            public CacheProperites()
            {
                this.IsTimeToLive = false;
            }

            
            
            public override string ToString()
            {
                return "name:" + this.Name + ", timeout:" + this.Timeout + ", maxHeapSize:" + this.Timeout + ", maxItemSize:" + this.Timeout + ",isTimeToLive:" + this.MaxHeapSize;
            }
        }
    }

}
