using Antlr4.Runtime.Misc;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Orm.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    public class EntityItem
    {
        public string FullObjectName { get;private set; }

        public IDataEntityType EntityType { get; private set; }

        public bool WithFullProperties { get; private set; } = false;

        public List<EntityItemProperty> Properties { get; private set; }

        public EntityItem Locale { get; private set; }

        public EntityItem ParentItem { get; private set; }

        public EntityItemJoinProperty JoinProperty { get;  set; }

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
                    fullname = (new StringBuilder(fullObjectName.Length+ 10)).Append(fullObjectName).Append('.').Append(p.Name).ToString();
                }
                else
                {
                    fullname = p.Name;
                }
                EntityItemProperty pi = new EntityItemProperty(item, p, entityTypeCache);
                if (p is ICollectionProperty){
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

                }else if (p is IComplexProperty)
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
                if (p.PropertyName==propertyName)
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
            if (!isLocale && this.Hint.SelectNullIfNotExistsProperty)
            {
                DynamicSimpleProperty property = new DynamicSimpleProperty(propertyName, typeof(String), null);
                property.Alias = "__NULL_FIELD";

                return new EntityItemProperty(this, (IDataEntityProperty)property, null);
            }
            throw new Exception($"{this.FullObjectName}属性不存在或未设置字段:{propertyName}");

      }
    }
}
