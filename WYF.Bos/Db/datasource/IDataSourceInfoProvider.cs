
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db.datasource
{
    public interface IDataSourceInfoProvider
    {
           DataSourceInfo GetDataSourceInfo(string tenantId, string routeKey, string accountId, bool readOnly)
        {
            //return DataSourceFactory.GetDataSource(tenantId, routeKey, accountId, readOnly);
            return null;
        }
  
       string GetSharedRouteKey(string tenantId, string routeKey, string accountId)
        {
            // return DataSourceFactory.GetSharedRouteKey(tenantId, routeKey, accountId);
            return null;
        }
    }
}
