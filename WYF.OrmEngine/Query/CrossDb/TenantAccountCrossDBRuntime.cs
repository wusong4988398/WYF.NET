using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.CrossDb
{
    public class TenantAccountCrossDBRuntime
    {
        public static string GetCrossDBTable(string table, string crossToRouteKey, bool withCrossDBObjectOrFilter)
        {
            //if (crossToRouteKey != null && CrossDBConfig.IsCrossDBEnable() &&
            //    ((CrossDBConfig.IsCrossDBEnableOnlyOrFilter() && withCrossDBObjectOrFilter) ||
            //     !CrossDBConfig.IsCrossDBEnableOnlyOrFilter()))
            //{
            //    RequestContextInfo rc = RequestContextInfo.Get();
            //    string key = $"{rc.TenantId}#{rc.AccountId}";
            //    if (!runtimeMap.ContainsKey(key))
            //    {
            //        runtimeMap[key] = new TenantAccountCrossDBRuntime();
            //    }
            //    TenantAccountCrossDBRuntime runtime = runtimeMap[key];
            //    return runtime.DoGetCrossDBTable(table, crossToRouteKey);
            //}

            return table;
        }
    }
}
