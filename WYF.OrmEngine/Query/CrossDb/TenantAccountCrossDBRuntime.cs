using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.CrossDb
{
    public class TenantAccountCrossDBRuntime
    {
        /// <summary>
        /// 根据给定的表名和路由键返回跨数据库表名。如果跨数据库功能未启用或不需要转换，则直接返回原始表名。
        /// </summary>
        /// <param name="table"></param>
        /// <param name="crossToRouteKey"></param>
        /// <param name="withCrossDBObjectOrFilter"></param>
        /// <returns></returns>
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
            if (crossToRouteKey.IsNullOrWhiteSpace()&& CrossDBConfig.IsCrossDBEnable)
            {

            }
            return table;
        }
    }
}
