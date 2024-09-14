using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.OrmEngine.Impl;

namespace WYF.OrmEngine.Query
{
    public class EntityItemLazyLoader
    {
        private ORMHint hint;

        public Dictionary<string, EntityItem> EntityItemMap { get; private set; } = new Dictionary<string, EntityItem>();

        private Dictionary<string, IDataEntityType> entityTypeCache;

        public EntityItemLazyLoader(ORMHint hint, QContext allCtx, Dictionary<string, IDataEntityType> entityTypeCache)
        {
            //this.hint = hint;
            //this.EntityItemMap = (allCtx == null) ? new Dictionary<string, EntityItem>() : allCtx.EntityItemLoader.entityItemMap;
            //this.entityTypeCache = entityTypeCache;
        }
        public EntityItem Load(IDataEntityType entityType, string asObjectName)
        {
            EntityItem ei = Load(asObjectName);
            if (ei == null)
            {
                ei = EntityItem.From(entityType, asObjectName, 1, this.entityTypeCache, this.hint);

                foreach (var item in ei.ToEntityItemMap())
                {
                    this.EntityItemMap[item.Key] = item.Value;
                }
            }
            return ei;
        }
        public EntityItem Load(string fullObjectName)
        {
            EntityItem ei = this.EntityItemMap.GetValueOrDefault(fullObjectName, null);
            if (ei == null)
            {
                int p = fullObjectName.LastIndexOf('.');
                if (p == -1)
                {
                    IDataEntityType entityType = ORMConfiguration.InnerGetDataEntityType(fullObjectName, this.entityTypeCache);
                    ei = EntityItem.From(entityType, fullObjectName, 1, this.entityTypeCache, this.hint);
                    foreach (var item in ei.ToEntityItemMap())
                    {
                        this.EntityItemMap[item.Key] = item.Value;
                    }
                    return ei;
                }
                string parentFullObjectName = fullObjectName.Substring(0, p);
                EntityItem parentEI = Load(parentFullObjectName);
                if (parentEI == null)
                    return null;
                ei = this.EntityItemMap.GetValueOrDefault(fullObjectName, null);
                if (ei != null)
                    return ei;

                string propertyName = fullObjectName.Substring(p + 1);
                IDataEntityProperty property = (IDataEntityProperty)parentEI.EntityType.Properties[propertyName];
                if (property == null)
                {
                    if (ORMConfiguration.IsEntryEntityType(parentEI.EntityType))
                        throw new Exception($"属性不存在或未设置字段：{propertyName}");

                    parentEI.ReplaceFullProperties();
                    property = (IDataEntityProperty)parentEI.EntityType.Properties[propertyName];

                }
                if (property is IComplexProperty)
                {
                    IDataEntityType entityType = ORMConfiguration.InnerGetBaseDataEntityType((IComplexProperty)property, this.entityTypeCache);
                    ei = EntityItem.From(parentEI, null, entityType, false, fullObjectName, 1, 1, this.entityTypeCache, this.hint);
                    ei.JoinProperty = new EntityItemJoinProperty(parentEI, property, this.entityTypeCache);
                    foreach (var item in ei.ToEntityItemMap())
                    {
                        this.EntityItemMap[item.Key] = item.Value;
                    }

                }
                else
                {
                    foreach (EntityItem subItem in parentEI.SubItems)
                    {
                        if (subItem.FullObjectName == fullObjectName)
                        {
                            ei = subItem;
                            break;
                        }
                    }

                }

            }

            return ei;
        }
    }
}
