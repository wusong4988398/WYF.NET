using WYF.DataEntity.Metadata;
using WYF.Bos.db;
using WYF.Bos.Orm.dataentity;
using WYF.Bos.Orm.DataManager;
using WYF.Bos.Orm.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public class DataManager : IDataManager
    {
        private IDataManager impl;

        public DataManager(IDataEntityType dataEntityType):this(dataEntityType, new DBRoute(dataEntityType.DBRouteKey))
        {
         
        }

        public DataManager(IDataEntityType dataEntityType, DBRoute dbRoute)
        {
            this.impl = (IDataManager)new DataManagerImplement(dataEntityType, dbRoute);
        }
        public object[] Read(object[] ids)
        {
            return this.impl.Read(ids);
        }

        public Object[] Read(ReadWhere where)
        {
            return this.impl.Read(where);
        }

        public void SetDataEntityType(IDataEntityType dataEntityType)
        {
            throw new NotImplementedException();
        }
    }
}
