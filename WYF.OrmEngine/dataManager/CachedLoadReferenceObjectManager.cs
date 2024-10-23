using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 继承自 LoadReferenceObjectManager，并扩展了其功能以支持缓存机制。
    /// 这个类的主要目的是优化数据实体引用对象的加载过程，通过使用缓存来减少对数据库的访问次数，从而提高性能
    /// </summary>
    public class CachedLoadReferenceObjectManager : LoadReferenceObjectManager
    {
        public CachedLoadReferenceObjectManager(IDataEntityType dt, bool onlyDbProperty)
            : base(dt, onlyDbProperty)
        {

        }

        protected override object[] Read(IDataEntityType dt, object[] oids)
        {
            object[] dataEntities = base.Read(dt, oids);
            var manager = new CachedLoadReferenceObjectManager(dt, false);
            manager.Load(dataEntities);

            ISimpleProperty pk = dt.PrimaryKey;
            var cacheManager = new DataEntityCacheManager(dt);
            if (pk != null && dataEntities.Length > 0)
            {
                cacheManager.Put(dataEntities);
            }
            //if (oids != null && oids.Length > dataEntities.Length)
            //{
            //    cacheManager.PutNotExistPKs(oids, dataEntities);
            //}
            return dataEntities;
        }

        protected override ICollection<DataEntityReferenceList> GetTasks(object[] dataEntities)
        {
            var tasks = base.GetTasks(dataEntities).ToList();
            foreach (var task in tasks)
            {
                DoTaskFromCache(new DataEntityCacheManager(task.DataEntityType), task);
            }
            return tasks;
        }

        private void DoTaskFromCache(IDataEntityCacheManager cacheManager, List<DataEntityReference> task)
        {
            var fromCacheObjects = new List<DynamicObject>(task.Count);
            var pkIds = new HashSet<object>(task.Count);
            foreach (var item in task)
            {
                pkIds.Add(item.Oid);
            }

            var cacheMap = cacheManager.Get(pkIds.ToArray());
            foreach (var item in task)
            {
                DynamicObject dataEntity = (DynamicObject)cacheMap[item.Oid];
                if (dataEntity != null)
                {
                    bool bShare = cacheManager.GetDataEntityType().CacheType == DataEntityCacheType.Share;
                    if (bShare)
                    {
                        item.SetDataEntity(dataEntity);
                    }
                    else
                    {
                        var dt = (DynamicObjectType)cacheManager.GetDataEntityType();
                        var newObj = new DynamicObject(dt);
                        foreach (var prop in cacheManager.GetDataEntityType().Properties)
                        {
                            var val = prop.GetValue((DynamicObject)item.DataEntity);
                            prop.SetValue(newObj, val);
                        }
                        item.SetDataEntity(newObj);
                        fromCacheObjects.Add(newObj);
                    }
                    fromCacheObjects.Add(dataEntity);
                }
                //else if (cacheManager.IsNotExistPK(item.Oid))
                //{
                //    item.SetDataEntity(null);
                //}
            }

            var manager = new CachedLoadReferenceObjectManager(cacheManager.GetDataEntityType(), base.GetOnlyDbProperty());
            var cacheObjects = fromCacheObjects.ToArray();
            if (bool.TryParse("QueryOrmError", out bool queryOrmError) && queryOrmError && cacheObjects.Length > 0 && cacheObjects[0] is DynamicObject o)
            {
                if (o.DataEntityType != cacheManager.GetDataEntityType())
                {
                    throw new InvalidOperationException($"缓存中对象属性为{cacheManager.GetDataEntityType().Name + cacheManager.GetDataEntityType().ExtendName + GetPropertiesString(cacheManager.GetDataEntityType())}");
                }
            }
            manager.Load(cacheObjects);
        }

        private string GetPropertiesString(IDataEntityType type)
        {
            var sb = new StringBuilder();
            foreach (var p in type.Properties)
            {
                sb.Append(p.Name).Append(',');
            }
            return sb.ToString();
        }
    }
}
