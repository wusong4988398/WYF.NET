using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache.Local
{
    public  interface ILocalSessionCacheRegionManger: ILocalMemoryCache
    {
        ILocalMemoryCache CreateOrReplaceCache(string type, CacheConfigInfo cacheConfig);

        ILocalMemoryCache GetOrCreateCache(string type, CacheConfigInfo cacheConfig);

        ILocalMemoryCache GetOrCreateCache(string type);

        ILocalMemoryCache GetCache(string type);

        void RemoveType(string type);

        void Destory();
    }
}
