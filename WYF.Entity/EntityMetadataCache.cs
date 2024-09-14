using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.Entity
{
    /// <summary>
    /// 实体元数据缓存服务类,用于获取实体元数据的各种元素信息
    /// </summary>
    public class EntityMetadataCache
    {
        private static IEntityMetaDataProvider _provider;

        public EntityMetadataCache()
        {
        }

        public static IEntityMetaDataProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = new EntityMetadataProvider();
                }
                return _provider;
            }
        }
        /// <summary>
        /// 返回实体元数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static MainEntityType? GetDataEntityType(string entityName)
        {
            MainEntityType dt = Provider.GetDataEntityType(entityName);

            return dt;
        }


        public static DynamicObjectType GetSubDataEntityType(string entityNumber, ICollection<string> properties)
        {
            return GetDataEntityType(entityNumber).GetSubEntityType(properties);
        }
    }
}
