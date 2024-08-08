using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public class DataEntityCacheManager : IDataEntityCacheManager
    {
        private string rootType;
        private IDataEntityType dt;
        private string regionKey;
        private string numberRegionKey;
        public DataEntityCacheManager(string tableName)
        {
            this.rootType = tableName.ToLower();
        }

        public DataEntityCacheManager(IDataEntityType dt)
        {
            if (dt == null)
                throw new ArgumentException("dt");
            if (dt.PrimaryKey == null)
                throw new ArgumentException("实体缺乏主键，无法缓存处理。");
            this.dt = dt;
            this.rootType = dt.Alias.ToLower();
            this.regionKey = GetSubType();
            this.numberRegionKey = this.regionKey + "_number";
        }

        private string GetSubType()
        {
            string region;
            if (this.dt is DynamicObjectType) {
                region = ((DynamicObjectType)this.dt).ExtendName;
            } else
            {
                region = this.dt.Name;
            }
            //RequestContext context = RequestContext.get();
            //return Instance.getClusterName() + "." + context.getAccountId() + "." + region;
            return "ClusterName" + "AccountId" + "." + region;
        }


        public Dictionary<object, object> Get(object[] pkArray)
        {
            throw new NotImplementedException();
        }

        public IDataEntityType GetDataEntityType()
        {
            throw new NotImplementedException();
        }
    }
}
