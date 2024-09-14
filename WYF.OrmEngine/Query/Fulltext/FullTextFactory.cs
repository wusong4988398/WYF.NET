using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.Fulltext
{
    public class FullTextFactory
    {
        public static IFullTextCustSyncQuery GetFullTextCustSyncQuery()
        {
            return GetFullTextCustSyncQuery("base");
        }
        public static IFullTextCustSyncQuery GetFullTextCustSyncQuery(string region)
        {
            //return new FullTextCustSyncQuery(region);
            return null;
        }
    }
}
