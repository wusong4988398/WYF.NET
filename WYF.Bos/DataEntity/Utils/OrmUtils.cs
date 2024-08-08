
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.Form.DataEntity.Utils
{
    public static  class OrmUtils
    {
    
        public static IDataEntityType GetDataEntityType(Type type)
        {
            return DataEntityType.GetDataEntityType(type);
        }


        public static object GetPrimaryKeyValue(this IDataEntityBase dataEntity, bool throwOnError = true)
        {
            if (dataEntity == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException("dataEntity");
                }
                return null;
            }
            ISimpleProperty primaryKey = dataEntity.DataEntityType.PrimaryKey;
            if (primaryKey != null)
            {
                return primaryKey.GetValue(dataEntity);
            }
            if (throwOnError)
            {
                throw new NotSupportedException(string.Format("实体类型{0}沒有定义主键，无法获取。", dataEntity.DataEntityType.Name));
            }
            return null;
        }


        public static T GetPrimaryKeyValue<T>(this IDataEntityBase dataEntity, bool throwOnError = true)
        {
            if (dataEntity == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException("dataEntity");
                }
                return default(T);
            }
            ISimpleProperty primaryKey = dataEntity.DataEntityType.PrimaryKey;
            if (primaryKey != null)
            {
                return (T)primaryKey.GetValue(dataEntity);
            }
            if (throwOnError)
            {
                throw new NotSupportedException(string.Format("实体类型{0}沒有定义主键，无法获取。", dataEntity.DataEntityType.Name));
            }
            return default(T);
        }
    }
}
