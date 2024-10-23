using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using WYF.DataEntity.Metadata.dynamicobject;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.OrmEngine.Impl;
using WYF.OrmEngine.Query.Multi;

namespace WYF.OrmEngine.Query
{
    /// <summary>
    /// 为了构建和管理数据库实体的层次结构而设计的。它能够处理简单的属性以及更复杂的嵌套关系和集合关系，并提供了递归地构建整个实体树的能力。
    /// 此外，它还支持局部化和一些高级的 ORM 功能，如属性选择的提示。
    /// </summary>
    public class EntityItem
    {
        /// <summary>
        /// 实体项的全名
        /// </summary>
        public string FullObjectName { get; private set; }
        /// <summary>
        /// 实体类型，代表了这个实体在数据库中的结构。
        /// </summary>
        public IDataEntityType EntityType { get; private set; }
        /// <summary>
        /// 标记是否包含所有属性。
        /// </summary>
        public bool WithFullProperties { get; private set; } = false;
        /// <summary>
        /// 属性列表，包含了该实体的所有属性信息。
        /// </summary>
        public List<EntityItemProperty> Properties { get; private set; }
        /// <summary>
        /// 用来表示本地化相关的实体项。
        /// </summary>
        public EntityItem Locale { get; private set; }
        /// <summary>
        /// 父级实体项，用于表达实体之间的关联。
        /// </summary>
        public EntityItem ParentItem { get; private set; }

        public EntityItemJoinProperty JoinProperty { get; set; }
        /// <summary>
        /// 用于描述一对多或多对多的关系
        /// </summary>
        public List<EntityItem> SubItems { get; private set; }

        public Dictionary<string, IDataEntityType> EntityTypeCache { get; private set; }

        public ORMHint Hint { get; private set; }

        public static EntityItem From(IDataEntityType entityType, string objectName, int maxLevel, Dictionary<string, IDataEntityType> entityTypeCache, ORMHint hint)
        {
            return From(null, null, entityType, true, objectName.ToLower(), 1, maxLevel, entityTypeCache, hint);
        }
        public static EntityItem From(EntityItem parentItem, EntityItemJoinProperty joinProperty, IDataEntityType entityType, bool withFullProperties, string fullObjectName, int curLevel, int maxLevel, Dictionary<string, IDataEntityType> entityTypeCache, ORMHint hint)
        {
            EntityItem item = new EntityItem();
            item.FullObjectName = fullObjectName;
            item.EntityType = entityType;
            item.WithFullProperties = withFullProperties;
            item.ParentItem = parentItem;
            item.JoinProperty = joinProperty;
            item.Properties = new List<EntityItemProperty>();
            item.SubItems = new List<EntityItem>();
            item.EntityTypeCache = entityTypeCache;
            item.Hint = hint;
            if (parentItem != null)
                parentItem.SubItems.Add(item);
            Dictionary<string, EntityItemProperty> map = new Dictionary<string, EntityItemProperty>();
            DataEntityPropertyCollection ps = entityType.Properties;
            for (int i = 0, n = ps.Count; i < n; i++)
            {
                string fullname;
                IDataEntityProperty p = (IDataEntityProperty)ps[i];
                if (!string.IsNullOrEmpty(fullObjectName))
                {
                    fullname = (new StringBuilder(fullObjectName.Length + 10)).Append(fullObjectName).Append('.').Append(p.Name).ToString();
                }
                else
                {
                    fullname = p.Name;
                }
                EntityItemProperty pi = new EntityItemProperty(item, p, entityTypeCache);
                if (p is ICollectionProperty)
                {
                    map[fullname] = pi;
                    ICollectionProperty cp = (ICollectionProperty)p;
                    EntityItemJoinProperty joinP = new EntityItemJoinProperty(item, p, entityTypeCache);
                    string entryAliasName;
                    if (!string.IsNullOrEmpty(fullObjectName))
                    {
                        entryAliasName = (new StringBuilder(fullObjectName.Length + 10)).Append(fullObjectName).Append('.').Append(cp.ItemType.Name).ToString();

                    }
                    else
                    {
                        entryAliasName = cp.ItemType.Name;
                    }
                    From(item, joinP, cp.ItemType, false, entryAliasName, curLevel, maxLevel, entityTypeCache, hint);

                }
                else if (p is IComplexProperty)
                {
                    if (!ORMUtil.IsDbIgnoreRefBaseData(p))
                    {
                        map[fullname] = pi;
                        if (curLevel < maxLevel)
                        {
                            IComplexProperty cp = (IComplexProperty)p;
                            EntityItemJoinProperty joinP = new EntityItemJoinProperty(item, p, entityTypeCache);
                            IDataEntityType type = ORMConfiguration.InnerGetBaseDataEntityType(cp, entityTypeCache);
                            From(item, joinP, type, false, (new StringBuilder(fullObjectName
      .Length + 10)).Append(fullObjectName).Append('.')
    .Append(cp.Name.ToLower()).ToString(), curLevel + 1, maxLevel, entityTypeCache, hint);

                        }
                    }
                }
                else if (!ORMUtil.IsDbIgnore(p))
                {
                    map[fullname] = pi;
                }

            }

            item.Properties.AddRange(map.Values);
            if (ORMConfiguration.IsMulBasedata(entityType))
            {
                IDataEntityType mulbdMainEntityType = entityType.Parent;
                ISimpleProperty iSimpleProperty = mulbdMainEntityType.PrimaryKey;
                DynamicSimpleProperty fakeProperty = new DynamicSimpleProperty(iSimpleProperty.Name, iSimpleProperty.GetType(), null);
                fakeProperty.Alias = iSimpleProperty.Alias;
                item.Properties.Add(new EntityItemProperty(item, (IDataEntityProperty)fakeProperty, entityTypeCache));
            }
            return item;
        }
        public Dictionary<string, EntityItem> ToEntityItemMap()
        {
            Dictionary<string, EntityItem> entityItemMap = new Dictionary<string, EntityItem>();
            entityItemMap[this.FullObjectName] = this;
            foreach (EntityItem subItem in this.SubItems)
            {
                Dictionary<string, EntityItem> subMap = subItem.ToEntityItemMap();
                foreach (var entry in subMap)
                {
                    if (!entityItemMap.ContainsKey(entry.Key))
                    {
                        entityItemMap[entry.Key] = entry.Value;
                    }
                }

            }
            return entityItemMap;
        }

        public EntityItemProperty GetPropertyItem(string propertyName)
        {
            return GetPropertyItem(propertyName, false);
        }

        public void ReplaceFullProperties()
        {
            this.WithFullProperties = true;
            this.EntityType = ORMConfiguration.InnerGetDataEntityType(this.EntityType.Name, this.EntityTypeCache);
            EntityItem fullEI = From(this.EntityType, this.FullObjectName, 1, this.EntityTypeCache, this.Hint);
            this.Locale = fullEI.Locale;
            this.SubItems.AddRange(fullEI.SubItems);
            Dictionary<string, EntityItemProperty> psMap = new Dictionary<string, EntityItemProperty>();
            foreach (EntityItemProperty p in this.Properties)
                psMap[p.PropertyName.ToLower()] = p;

            foreach (EntityItemProperty p in fullEI.Properties)
            {
                if (!psMap.ContainsKey(p.PropertyName.ToLower()))
                {
                    this.Properties.Add(p);
                    p.EntityItem = this;
                }
            }
        }

        private EntityItemProperty GetPropertyItem(string propertyName, bool isLocale)
        {
            foreach (EntityItemProperty p in this.Properties)
            {
                if (p.PropertyName == propertyName)
                    return p;
            }
            if (this.Locale != null)
                try
                {
                    return this.Locale.GetPropertyItem(propertyName, true);
                }
                catch (Exception exception) { }
            if (!this.WithFullProperties && ORMConfiguration.IsBasedata(this.EntityType))
            {
                ReplaceFullProperties();
                return GetPropertyItem(propertyName);
            }
            if (!isLocale && this.Hint.IsSelectNullIfNotExistsProperty)
            {
                DynamicSimpleProperty property = new DynamicSimpleProperty(propertyName, typeof(String), null);
                property.Alias = "__NULL_FIELD";

                return new EntityItemProperty(this, (IDataEntityProperty)property, null);
            }
            throw new Exception($"{this.FullObjectName}属性不存在或未设置字段:{propertyName}");

        }
    }
}
