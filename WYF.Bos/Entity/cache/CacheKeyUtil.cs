using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.cache
{
    public  class CacheKeyUtil
    {
        public static String GetAcctId()
        {
            return "jnpf";
        }

        public static TimeSpan GetPageCacheKeyTimeout()
        {
            return TimeSpan.FromMinutes(60);
        }
    }
}
