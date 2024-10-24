using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache
{
    public abstract class CacheFactory
    {
        private static volatile CacheFactory commonCacheFactory = null;
        private static readonly object syncLock = new object();


        public static CacheFactory GetCommonCacheFactory()
        {
            if (commonCacheFactory == null)
            {
                lock (syncLock)
                {
                    if (commonCacheFactory == null)
                    {
                        commonCacheFactory = (new CacheFactoryBuilder()).Build();
                    }
                }
            }
            return commonCacheFactory;
        }

        public abstract ILocalMemoryCache GetOrCreateLocalMemoryCache(string region, string type);

        public abstract ILocalMemoryCache GetOrCreateLocalMemoryCache(string region, string type, CacheConfigInfo cacheConfig);

        public abstract IDistributeSessionlessCache GetDistributeSessionlessCache();

        public abstract IDistributeSessionlessCache GetDistributeSessionlessCache(string region);
    }
}
