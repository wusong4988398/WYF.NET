using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.Entity.Cache;

namespace WYF.Entity
{
    public class EntityMetadataLocalCache
    {
     private static readonly CacheConfigInfo cacheConfig = new CacheConfigInfo();
     static EntityMetadataLocalCache()
        {
            cacheConfig.Timeout = 43200;
            cacheConfig.MaxMemSize = 10000;
        }
    public static MainEntityType GetDataEntityType(string entitynumber)
        {
            string key = MakeCacheKey(RuntimeMetaType.Entity, entitynumber);
            return (MainEntityType)GetLocalCache().Get(key);
        }

        private static string MakeCacheKey(RuntimeMetaType type, string entitynumber)
        {
            string enumName = Enum.GetName(typeof(RuntimeMetaType), type);
            return (enumName+ "." + entitynumber).ToLower();
        }
        public static void PutDataEntityType(MainEntityType dt)
        {
            String key = MakeCacheKey(RuntimeMetaType.Entity, dt.Name);
            GetLocalCache().Put(key, dt);
        }
        private static ILocalMemoryCache GetLocalCache()
        {
            return CacheFactory.GetCommonCacheFactory().GetOrCreateLocalMemoryCache(CacheKeyUtil.GetAcctId(), "EntityMetadata", cacheConfig);
        }




    }
}
