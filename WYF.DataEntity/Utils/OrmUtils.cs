
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WYF.Bos.DataEntity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.DataEntity.Utils
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

        public static void DataEntityWalker(List<object> dataEntities, IDataEntityType dt, Action<DataEntityWalkerEventArgs> callback, bool onlyDbProperty)
        {
            DataEntityWalkerEventArgs.DataEntityWalker(dataEntities, dt, callback, onlyDbProperty);
        }

        public static object Clone(IDataEntityBase dataEntity, bool onlyDbProperty, bool clearPrimaryKeyValue)
        {
            return (new CloneUtils(onlyDbProperty, clearPrimaryKeyValue)).Clone(dataEntity);
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
        public static  void Sync<SourceT, TargetT>(ICollection<SourceT> sourceList, ICollection<TargetT> targetList, ListSyncFunction<SourceT, TargetT> syncFunction, bool callUpdateFuncWhenCreated)
        {
            ListSync.Sync(sourceList, targetList, syncFunction, callUpdateFuncWhenCreated);
        }
    
    }
}
