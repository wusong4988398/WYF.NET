using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Metadata.Dao
{
    public class MetaCacheUtils
    {
        public static string GetDistributeCache(string number, string key, int metaType)
        {
            //DistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache();
            // ICache cache= App.GetService<ICache>();
            ICache cache = null;


            //string accountId = CacheKeyUtil.getAcctId();

            //string type = string.format("%s_meta_%s", new Object[] { accountId, number });
            //return (string)cache.get(type, getCacheKey(key, metaType));

            return "";
        }
    }
}
