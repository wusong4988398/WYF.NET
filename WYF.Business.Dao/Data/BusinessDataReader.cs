using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.OrmEngine;
using WYF.OrmEngine.dataManager;


namespace WYF.Data
{
    /// <summary>
    /// 单据读取实现
    /// </summary>
    public class BusinessDataReader
    {

        public static DynamicObject LoadSingle(object pk, DynamicObjectType type)
        {
            return (DynamicObject)Read(pk, (IDataEntityType)type, true);
        }
        public static object Read(object pk, IDataEntityType type, bool loadReferenceData)
        {
            return Read(pk, type, loadReferenceData, false);
        }

        public static DynamicObject[] Load(object[] pkArray, DynamicObjectType type, bool loadReferenceData)
        {
            IDataManager dataManager = DataManagerUtils.GetDataManager((IDataEntityType)type);
            DynamicObject[] array = (DynamicObject[])dataManager.Read(pkArray);
            if (loadReferenceData)
            {
                CachedLoadReferenceObjectManager cachedLoadReferenceObjectManager = new CachedLoadReferenceObjectManager((IDataEntityType)type, false);
                cachedLoadReferenceObjectManager.Load((object[])array);
            }
            return array;
        }


        public static IDictionary<object, DynamicObject> LoadFromCache(object[] pkArray, DynamicObjectType dt)
        {
            return LoadFromCache(pkArray, dt, true);
        }

        public static Dictionary<object, DynamicObject> LoadFromCache(object[] pkArray, DynamicObjectType dt, bool loadReferenceData)
        {
            var oDynamicObject = new Dictionary<string, DynamicObject>();
            var notFindFromCachePkArray = new List<object>();
            var cacheManager = new DataEntityCacheManager(dt as IDataEntityType);
            var cacheMap = cacheManager.Get(pkArray);
            var iSimpleProperty = dt.PrimaryKey;

            foreach (var pk in pkArray)
            {
                if (cacheMap.TryGetValue(pk, out var dataEntity) && dataEntity is DynamicObject dynamicEntity)
                {
                    var bShare = dt.CacheType == DataEntityCacheType.Share;
                    var pkValue = iSimpleProperty.GetValueFast(dynamicEntity);

                    if (bShare && pkValue != null)
                    {
                        oDynamicObject[pkValue.ToString()] = dynamicEntity;
                    }
                    else
                    {
                        var newobj = dt.CreateInstance() as DynamicObject;
                        foreach (var prop in dt.Properties)
                        {
                            var val = prop.GetValue(dynamicEntity);
                            prop.SetValue(newobj, val);
                        }
                        pkValue = iSimpleProperty.GetValueFast(newobj);
                        if (pkValue != null)
                        {
                            oDynamicObject[pkValue.ToString()] = newobj;
                        }
                    }
                }
                else
                {
                    notFindFromCachePkArray.Add(pk);
                }
            }

            if (notFindFromCachePkArray.Count > 0)
            {
                var notCacheObjects = Load(notFindFromCachePkArray.ToArray(), dt, false).Cast<DynamicObject>().ToArray();
                cacheManager.Put(notCacheObjects.Cast<object>().ToArray());

                foreach (var o in notCacheObjects)
                {
                    var pkValue = iSimpleProperty.GetValueFast(o);
                    if (pkValue != null)
                    {
                        oDynamicObject[pkValue.ToString()] = o;
                    }
                }
            }

            var orderedDynamicObject = new Dictionary<object, DynamicObject>();
            foreach (var pk in pkArray)
            {
                if (pk != null && oDynamicObject.TryGetValue(pk.ToString(), out var dynamicObject))
                {
                    var pkValue = iSimpleProperty.GetValueFast(dynamicObject);
                    if (pkValue != null)
                    {
                        orderedDynamicObject[pkValue] = dynamicObject;
                    }
                }
            }

            var results = orderedDynamicObject.Values.ToArray();
            if (loadReferenceData && results.Length > 0)
            {
                LoadReference(results, dt as IDataEntityType);
            }

            return orderedDynamicObject.ToDictionary(x => x.Key, x => x.Value);
        }

        public static void LoadReference(object[] dataEntitys, IDataEntityType type)
        {
            CachedLoadReferenceObjectManager cachedLoadReferenceObjectManager = new CachedLoadReferenceObjectManager(type, false);
            cachedLoadReferenceObjectManager.Load(dataEntitys);
        }


        private static object Read(object pk, IDataEntityType type, bool loadReferenceData, bool selectHeadOnly)
        {
            IDataManager dataManager = DataManagerUtils.GetDataManager(type);
            dataManager.IsSelectHeadOnly = selectHeadOnly;

            object obj = dataManager.Read(pk);
            if (loadReferenceData&& obj != null)
            {
                CachedLoadReferenceObjectManager manager = new CachedLoadReferenceObjectManager(type, false);
                manager.Load(obj);
            }
            return obj;
        }
    }
}
