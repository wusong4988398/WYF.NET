﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.OrmEngine.DataManager;

namespace WYF.Data
{
    public class BusinessDataReader
    {
        public static DynamicObject[] Load(Object[] pkArray, DynamicObjectType type, bool loadReferenceData)
        {
            //IDataManager dataManager = DataManagerUtils.GetDataManager((IDataEntityType)type);
            //DynamicObject[] array = (DynamicObject[])dataManager.Read(pkArray);
            //if (loadReferenceData)
            //{
            //    CachedLoadReferenceObjectManager cachedLoadReferenceObjectManager = new CachedLoadReferenceObjectManager((IDataEntityType)type, false);
            //    cachedLoadReferenceObjectManager.Load((Object[])array);
            //}
            //return array;
            return null;
        }
    }
}