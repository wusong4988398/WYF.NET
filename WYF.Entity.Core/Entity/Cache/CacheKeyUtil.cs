using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Common;

namespace WYF.Entity.Cache
{
    public class CacheKeyUtil
    {
        public static string GetAcctId()
        {
            string accid = Instance.GetClusterName() + "." + RequestContext.Get().AccountId;
            return accid;

        }
        public static TimeSpan GetPageCacheKeyTimeout()
        {
            return TimeSpan.FromMinutes(60);
        }
    }
}
