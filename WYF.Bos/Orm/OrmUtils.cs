
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm
{
    public static class OrmUtils
    {
        public static void DataEntityWalker(List<Object> dataEntities, IDataEntityType dt, Action<DataEntityWalkerEventArgs> callback, bool onlyDbProperty)
        {
            DataEntityWalkerEventArgs.DataEntityWalker(dataEntities, dt, callback, onlyDbProperty);
        }


        public static Object Clone(IDataEntityBase dataEntity, bool onlyDbProperty, bool clearPrimaryKeyValue)
        {
            return (new CloneUtils(onlyDbProperty, clearPrimaryKeyValue)).Clone(dataEntity);
        }

    }
}
