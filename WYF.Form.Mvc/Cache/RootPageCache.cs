using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.Entity.Cache;

namespace WYF.Mvc.Cache
{
    public class RootPageCache
    {
        private static IDistributeSessionlessCache _cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache("form-pagecache");
        public static void AddPageId(string rootPageId, string pageId)
        {
            long currenttimemillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            if (pageId.StartsWith("root"))
            {
                string rootPageKey = GetSessionKey();
                _cache.Put(rootPageKey, pageId, currenttimemillis.ToString(),
                        CacheKeyUtil.GetPageCacheKeyTimeout());
            }
            else
            {
                String rootPageKey = CacheKeyUtil.GetAcctId() + ".root." + rootPageId;
                _cache.Put(rootPageKey, pageId, currenttimemillis.ToString(),
                        CacheKeyUtil.GetPageCacheKeyTimeout());
            }
        }


        private static string GetSessionKey()
        {
            //return GetSessionKey(RequestContext.get().getGlobalSessionId());
            return GetSessionKey("session123456789");//这里写死
        }

        private static string GetSessionKey(string sessionId)
        {
            return "session." + sessionId;
        }
    }
}
