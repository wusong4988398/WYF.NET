using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Orm.DataManager;
using WYF.Bos.Orm;
using WYF.Bos.Orm.query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.Orm.datamanager;

namespace WYF.Bos.Data
{
    public class BusinessDataReader
    {
        public static DynamicObject[] Load(Object[] pkArray, DynamicObjectType type, bool loadReferenceData)
        {
            IDataManager dataManager = DataManagerUtils.GetDataManager((IDataEntityType)type);
            DynamicObject[] array = (DynamicObject[])dataManager.Read(pkArray);
            if (loadReferenceData)
            {
                CachedLoadReferenceObjectManager cachedLoadReferenceObjectManager = new CachedLoadReferenceObjectManager((IDataEntityType)type, false);
                cachedLoadReferenceObjectManager.Load((Object[])array);
            }
            return array;
        }
    }
}
