using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine.db
{
    public class DBRoute
    {
        public string RouteKey { get; set; }
        private static IDictionary<string, DBRoute> _sharedDBRouteMap = new ConcurrentDictionary<string, DBRoute>();
        private IDictionary<string, string> tableRouteMap;
        public DBRoute(string routeKey)
        {
            this.RouteKey = (routeKey == null) ? "" : routeKey.Trim().ToLower();
        }
        public static DBRoute Of(string routeKey)
        {
            routeKey = (routeKey == null) ? "" : routeKey.Trim().ToLower();
            DBRoute route = _sharedDBRouteMap.GetOrDefault(routeKey);
            if (route == null)
                route = _sharedDBRouteMap.GetOrAdd(routeKey, () => {
                    return new DBRoute(routeKey);
                });
            //route = _sharedDBRouteMap.ComputeIfAbsent(routeKey, key-> new DBRoute(key));
            return route;
        }


        public bool HasEmptyTableRouteKey()
        {
            return (this.tableRouteMap == null || this.tableRouteMap.Count == 0);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetTableRouteKey(string table)
        {
            //if (this.tableRouteMap != null)
            //{
            //    table = TableName.of(table.Trim()).getOriginalName();
            //    string routeKey = this.tableRouteMap.get(table);
            //    if (routeKey != null)
            //        return routeKey;
            //}
            return this.RouteKey;
        }

    }
}
