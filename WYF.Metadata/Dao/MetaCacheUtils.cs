using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.Entity.Cache;
namespace WYF.Metadata.Dao
{
    public class MetaCacheUtils
    {
        public static string GetDistributeCache(string number, string key, int metaType)
        {
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache();
            string accountId = CacheKeyUtil.GetAcctId();
            string type = $"{accountId}_meta_{number}";
            return cache.Get(type, GetCacheKey(key, metaType));
        }

        public static void PutDistributeCache(string number, string key, int metaType, string val)
        {
            if (val == null)
                return;
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache();
            String accountId = CacheKeyUtil.GetAcctId();

            string cacheKey = $"{accountId}_meta_{number}";

            cache.Put(cacheKey, GetCacheKey(key, metaType), val);
        }

        public static string GetFormMetaVersion(string number)
        {
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache();
            string accountId = CacheKeyUtil.GetAcctId();
            string cacheKey = $"{accountId}_metaversion_{number}";
            return cache.Get(cacheKey);
        }

        public static void SetFormMetaVersion(string number, string version)
        {
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache();
            String accountId = CacheKeyUtil.GetAcctId();
            string cacheKey = $"{accountId}_metaversion_{number}";
            cache.Put(cacheKey, version, TimeSpan.FromSeconds(5000));
        }


        private static string GetCacheKey(string key, int metaType)
        {

            string cacheKey = $"_{metaType}_{key}";
            return cacheKey;
            
        }




    }
}
