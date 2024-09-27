using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.OrmEngine.Impl;
using WYF.OrmEngine.Query.Multi;
using static IronPython.Modules.PythonIterTools;

namespace WYF.OrmEngine.Query
{
    public sealed class QContext
    {
        private static bool ignoreNoneSelectObject = true;

        public Dictionary<string, IDataEntityType> EntityTypeCache { get; private set; }

        public EntityItem EntityItem { get; private set; }

        private readonly HashSet<string> joinAliasSet = new HashSet<string>(5);

        public List<JoinTableInfo> JoinTableList { get; set; } = new List<JoinTableInfo>(5);

        private readonly Dictionary<string, string> simpleEntityAliasMap;

        public HashSet<string> CurrentSelectObjectSet { get; set; } = new HashSet<string>();

        public Dictionary<string, List<PropertyField>> SelectFieldMap { get; set; } = new Dictionary<string, List<PropertyField>>();

        private readonly Dictionary<string, List<PropertyField>> performJoinFieldMap = new Dictionary<string, List<PropertyField>>();

        public EntityItemLazyLoader EntityItemLoader { get; private set; }

        public ORMHint ORMHint => ormHint;

        public IDistinctable Distinctable => distinctable;

        public Dictionary<string, string> SimpleEntityAliasMap => simpleEntityAliasMap;

        private readonly string objectFullName;



        private readonly ORMHint ormHint;

        private readonly IDistinctable distinctable;

        private bool silenceHandleAllFilterAndOrderAndGroupBy;

        private readonly HashSet<EntityItem> innerJoinFilterEntityItemSet;

        private Dictionary<string, string> replaceEntityAliasMap;

        public QContext(IDataEntityType entityType, string objectFullName, PropertyField[] fields, Dictionary<string, IDataEntityType> entityTypeCache, ORMHint ormHint, IDistinctable distinctable, bool silenceHandleAllFilterAndOrderAndGroupBy, Dictionary<string, string> simpleEntityAliasMap, QContext allCtx)
        {
            this.EntityTypeCache = entityTypeCache;
            this.objectFullName = objectFullName;
            this.ormHint = ormHint;
            this.distinctable = distinctable;
            this.simpleEntityAliasMap = simpleEntityAliasMap;
            GetSimpleEntityAlias(objectFullName);
            this.EntityItemLoader = new EntityItemLazyLoader(ormHint, allCtx, entityTypeCache);
            if (allCtx == null)
            {
                this.EntityItem = this.EntityItemLoader.Load(entityType, objectFullName);
                this.innerJoinFilterEntityItemSet = new HashSet<EntityItem>();
            }
            else
            {
                this.EntityItem = allCtx.EntityItem;
                this.innerJoinFilterEntityItemSet = allCtx.innerJoinFilterEntityItemSet;
            }

            AddField2map(objectFullName, null, this.SelectFieldMap, true);
            this.silenceHandleAllFilterAndOrderAndGroupBy = false;
            for (int i = 0, n = fields.Count(); i < n; i++)
            {
                PropertyField field = fields[i];
                PutField(field, true, false);
            }
            this.silenceHandleAllFilterAndOrderAndGroupBy = silenceHandleAllFilterAndOrderAndGroupBy;

        }


        public void AddField2map(string fullObjectName, PropertyField field, Dictionary<string, List<PropertyField>> map, bool add2CurrentSelectObject)
        {
            if (add2CurrentSelectObject)
            {
                bool ignore = false;
                if (field == null || field.Field == null)
                {
                    if (ignoreNoneSelectObject)
                        ignore = true;
                }
                if (!ignore)
                    this.CurrentSelectObjectSet.Add(fullObjectName);
            }

            EntityItem ei = GetEntityItem(fullObjectName);
            if (ei == null || ORMConfiguration.IsEntryEntityType(ei.EntityType))
                return;

            List<PropertyField> list = null;
            if (map.TryGetValue(fullObjectName, out list) == false)
            {
                list = new List<PropertyField>();
                map[fullObjectName] = list;
            }

            if (field != null)
                list.Add(field);

            int dot = fullObjectName.LastIndexOf('.');
            if (dot != -1)
                AddField2map(fullObjectName.Substring(0, dot), null, map, false);
        }

        public PropertyField PutPerformJoinField(PropertyField field)
        {
            return PutPerformJoinField(field, null, null);
        }


        public List<PropertyField> GetPerformJoinFieldList(string fullObjectName)
        {
            return this.performJoinFieldMap.GetValueOrDefault(fullObjectName);
        }

        public PropertyField PutPerformJoinField(PropertyField field, QFilter joinFilter, String joinFilterProperty)
        {
            List<PropertyField> list = this.performJoinFieldMap.GetValueOrDefault(field.FullName.ToLower(), null);
            if (list != null)
                foreach (PropertyField pf in list)
                {
                    if (pf.IsSameWith(field))
                        return pf;
                }
            return PutJoinField(field, false, true, joinFilter, joinFilterProperty);
        }

        private PropertyField PutJoinField(PropertyField field, bool add2SelectObjectMap, bool add2PerformJoinFieldList, QFilter joinFilter, string joinFilterProperty)
        {
            if (joinFilter != null)
            {
                string fullObjectName = field.FullObjectName.ToLower();
                EntityItem ei = GetEntityItem(fullObjectName);
                if (ei == null)
                {
                    fullObjectName = fullObjectName.Substring(fullObjectName.IndexOf('.') + 1);
                    ei = GetEntityItem(fullObjectName);
                    if (ei != null)
                    {
                        field.FullObjectName = fullObjectName;
                        if (ei.JoinProperty == null)
                        {
                            string property;
                            EntityItem parentEntityItem;
                            int dot = joinFilterProperty.LastIndexOf('.');
                            if (dot != -1)
                            {
                                parentEntityItem = GetEntityItem(joinFilterProperty.Substring(0, dot));
                                if (parentEntityItem == null) throw new ArgumentException($"父实体不存在, name={joinFilterProperty}");
                                property = joinFilterProperty.Substring(dot + 1);
                            }
                            else
                            {
                                parentEntityItem = GetEntityItem(this.objectFullName);
                                if (parentEntityItem == null) throw new ArgumentException($"父实体不存在, name={joinFilterProperty}");
                                property = joinFilterProperty;

                            }
                            String eiJoinPropertyName = joinFilter.Value.ToString().Substring(fullObjectName.Length + 1);
                            ei.JoinProperty = new EntityItemJoinProperty(parentEntityItem, parentEntityItem.GetPropertyItem(property).PropertyType, ei.GetPropertyItem(eiJoinPropertyName).PropertyType);
                        }
                    }
                }
                if (joinFilter.GetJoinHint() == ORMHint.JoinHint.INNER)
                    this.innerJoinFilterEntityItemSet.Add(ei);
            }
            return PutField(field, add2SelectObjectMap, add2PerformJoinFieldList, (joinFilter == null) ? ORMHint.JoinHint.DEFAULT : joinFilter.GetJoinHint());
        }



        private PropertyField PutField(PropertyField field, bool add2SelectObjectMap, bool add2PerformJoinFieldList, ORMHint.JoinHint filterJoinHint)
        {
            string fullObjectName = field.FullObjectName.ToLower();
            bool isExpress = field.IsExpress;
            EntityItem ei = GetEntityItem(fullObjectName);
            if (ei == null)
            {
                fullObjectName = fullObjectName.Substring(fullObjectName.IndexOf('.') + 1);
                field.FullObjectName = fullObjectName;
                ei = GetEntityItem(fullObjectName);
            }
            if (ei == null && !isExpress)
            {
                throw new Exception($"在对象{this.objectFullName}中找不到子对象{fullObjectName},子对象在元数据中未定义，或两者非父子关系(过滤条件不正确：{fullObjectName}的属性{field.Name}与{this.objectFullName}的属性之间不允许有or关系)");
            }
            if (ei != null)
            {
                bool maybeJoinBD = !fullObjectName.Equals(this.objectFullName);
                if (ORMConfiguration.IsEntryEntityType(ei.EntityType))
                {
                    EntityItem joinEI;
                    maybeJoinBD = false;
                    do
                    {
                        joinEI = ei;
                        AddJoin(joinEI, JoinTableTypeEnum.entry, false, ORMHint.JoinHint.LEFT);
                        int dot = fullObjectName.LastIndexOf('.');
                        if (dot == -1)
                            break;
                        fullObjectName = fullObjectName.Substring(0, dot);
                        joinEI = GetEntityItem(fullObjectName);
                    } while (joinEI != null && ORMConfiguration.IsEntryEntityType(joinEI.EntityType));


                }
                if (!isExpress)
                {
                    PropertyField newPut = PutPropertyItem(field, ei.GetPropertyItem(field.Name));
                    if (newPut != null)
                        return PutField(newPut, true, true);
                    IDataEntityProperty propertyType = field.PeropertyType;
                    string tableGroup = propertyType.TableGroup;
                    if (!string.IsNullOrEmpty(tableGroup))
                    {
                        maybeJoinBD = false;
                        EnsureJoined(ei);
                        string groupAlias = ei.FullObjectName + '_' + tableGroup;
                        field.EntityAlias = groupAlias;
                        bool isEntry = ORMConfiguration.IsEntryEntityType(propertyType.Parent);
                        AddJoinExtend(fullObjectName, ei, propertyType, tableGroup, groupAlias, isEntry);
                    }
                    if (maybeJoinBD)
                    {
                        AddJoin(ei, JoinTableTypeEnum.basedata, ORMConfiguration.IsMulBasedata(ei.EntityType), filterJoinHint);
                    }
             
                    else if (ORMConfiguration.IsMulBasedataProp(propertyType) && field.IsInnerField)
                    {
                        String mulFullObjectName;
                        if (ORMConfiguration.IsEntryEntityType(propertyType.Parent))
                        {
                            IDataEntityType entryParentDT = propertyType.Parent.Parent;
                            if (entryParentDT != null && ORMConfiguration.IsEntryEntityType(entryParentDT))
                            {
                                mulFullObjectName = fullObjectName + '.' + entryParentDT.Name + '.' + propertyType.Parent.Name + '.' + propertyType.Name;
                            }
                            else
                            {
                                mulFullObjectName = fullObjectName + '.' + propertyType.Parent.Name + '.' + propertyType.Name;
                            }
                        }
                        else
                        {
                            mulFullObjectName = fullObjectName + '.' + propertyType.Name;
                        }
                        EntityItem mulEI = GetEntityItem(mulFullObjectName);
                        AddJoin(mulEI, JoinTableTypeEnum.basedata, true, filterJoinHint);
                        EntityItemProperty pi = mulEI.GetPropertyItem(mulEI.EntityType.PrimaryKey.Name);
                        PutPropertyItem(field, pi);
                        String fieldName = mulEI.EntityType.PrimaryKey.Alias;
                        //((DynamicProperty)propertyType).setAlias(fieldName);
                        ((DynamicProperty)propertyType).Alias = fieldName;
                        PropertyField mainPKField = new PropertyField(mulFullObjectName + '.' + mulEI.EntityType.PrimaryKey.Name);
                        PutField(mainPKField, true, false);
                    }
                }
            }

            if (field.IsExpress)
                PerformJoin(field.PropertySegExpress);
            return field;
        }

        private void PerformJoin(PropertySegExpress pse)
        {
            List<string> pns = pse.GetFullPropertyNames();
            if (pns.Count() > 0)
            {
                string rootObjName = GetRootObjName();
                foreach (string propertyName in pns)
                {
                    string fullPropertyName = rootObjName + "." + propertyName;
                    PropertyField pf = PutField(new PropertyField(fullPropertyName), false, true);
                    pse.PutFieldMap(fullPropertyName, pf);
                }
            }
        }

        private string GetRootObjName()
        {
            string rootObjName;
            int dot = this.objectFullName.IndexOf('.');
            if (dot == -1)
            {
                rootObjName = this.objectFullName;
            }
            else
            {
                rootObjName = this.objectFullName.Substring(0, dot);
            }
            return rootObjName;
        }

        public PropertyField AddSelectField(PropertyField field)
        {
            return PutField(field, true, false);
        }

        public string GetOrderBy(params OrderByInfo[] orderBys)
        {
            StringBuilder sb = new StringBuilder();
            foreach (OrderByInfo orderBy in orderBys)
            {
                PerformJoin(orderBy.PropertySegExpress);
                sb.Append(orderBy.ToOrderByString(this));
                sb.Append(',');
            }
            if (sb.Length > 0)
                sb.Length--;
            return sb.ToString();
        }

        public string GetSelects(string defaultEntityAlias, params PropertyField[] selectFields)
        {
            StringBuilder sql = new StringBuilder(256);
            foreach (PropertyField propertyField in selectFields)
            {
                if (!propertyField.IsExpress && propertyField.EntityAlias.Length == 0)
                    propertyField.EntityAlias = defaultEntityAlias;
                sql.Append(propertyField.ToSelectField(true, this));
                sql.Append(", ");
            }
            if (sql.Length == 0)
            {
                sql.Append("1 INNER__NONE");
            }
            else
            {
                sql.Length -= 2;
            }
            return sql.ToString();
        }
        public string GetGroupBy(params GroupByInfo[] groupBys)
        {
            StringBuilder sb = new StringBuilder();
            foreach (GroupByInfo groupBy in groupBys)
            {
                PerformJoin(groupBy.PropertySegExpress);
                sb.Append(groupBy.ToGroupByString(this));
                sb.Append(',');
            }
            if (sb.Length > 0)
                sb.Length--;
            return sb.ToString();
        }

        private void EnsureJoined(EntityItem ei)
        {
            String alias = ei.FullObjectName.ToLower();
            if (alias.Equals(this.objectFullName))
                return;
            if (!this.joinAliasSet.Contains(alias))
            {
                JoinTableTypeEnum joinTableType = ORMConfiguration.IsBasedata(ei.EntityType) ? JoinTableTypeEnum.basedata : JoinTableTypeEnum.entry;
                AddJoin(ei, joinTableType, false, ORMHint.JoinHint.LEFT);
                string pproperty = ei.FullObjectName + '.' + ei.EntityType.PrimaryKey.Name;
                PutField(new PropertyField(pproperty), false, false);
            }
        }

        private PropertyField PutPropertyItem(PropertyField field, EntityItemProperty pi)
        {
            if (ORMConfiguration.IsEntryProPropertyType(pi.PropertyType))
            {
                IDataEntityType itemType = ((ICollectionProperty)pi.PropertyType).ItemType;
                return new PropertyField(pi.EntityItem.FullObjectName + '.' + field.Name, itemType
                    .PrimaryKey.Name, field.Alias);
            }
            field.EntityAlias = pi.EntityItem.FullObjectName;
            field.EntityType = pi.EntityItem.EntityType;
            field.PeropertyType = pi.PropertyType;

            if (ORMConfiguration.IsMulBasedataProp(pi.PropertyType))
            {
                field.Field = pi.PropertyType.Parent.PrimaryKey.Alias;
            }
            else
            {
                field.Field = pi.PropertyType.Alias;
            }
            field.PropertyItem = pi;
            return null;
        }

        private void AddJoin(EntityItem joinEntityItem, JoinTableTypeEnum joinTableType, bool baseDataAndMul, ORMHint.JoinHint filterJoinHint)
        {
            if (joinEntityItem.JoinProperty == null)
                return;
            string joinAlias = joinEntityItem.FullObjectName;
            if (this.joinAliasSet.Contains(joinAlias))
                return;
            this.joinAliasSet.Add(joinAlias);
            JoinTableInfo join = new JoinTableInfo(joinEntityItem, joinTableType);
            join.Table = joinEntityItem.EntityType.Alias;
            if (this.innerJoinFilterEntityItemSet.Contains(joinEntityItem))
            {
                join.Join = ORMHint.JoinHint.INNER;

            }
            else if (ORMHint.IsInnerJoinConfigured(join.Table))
            {
                join.Join = ORMHint.JoinHint.INNER;
            }
            else
            {
                join.Join = this.ormHint.joinHint(ORMUtil.GetFullObjNameWithoutRoot(joinAlias));
            }
            join.TableAlias = joinAlias;
            if (joinTableType == JoinTableTypeEnum.entry)
            {
                if (joinEntityItem.ParentItem != null)
                {
                    join.Field = joinEntityItem.ParentItem.EntityType.PrimaryKey.Alias;
                }
                else
                {
                    
                    join.Field = joinEntityItem.JoinProperty.SubJoinProperty.Alias;
                }
            }
            else
            {
                if (!baseDataAndMul)
                    baseDataAndMul = ORMConfiguration.IsMulBasedataProp(joinEntityItem.JoinProperty.ParentJoinProperty);
                if (baseDataAndMul)
                {
                    if (joinEntityItem.ParentItem != null)
                    {
                        join.Field = joinEntityItem.ParentItem.EntityType.PrimaryKey.Alias;
                    }
                    else
                    {
                        join.Field = joinEntityItem.JoinProperty.SubJoinProperty.Alias;
                    }
                }
                else
                {
                    join.Field = joinEntityItem.JoinProperty.SubJoinProperty.Alias;
                }
            }
            string joinField = joinEntityItem.JoinProperty.ParentJoinProperty.Alias;
            if (string.IsNullOrEmpty(joinField))
                joinField = joinEntityItem.ParentItem.EntityType.PrimaryKey.Alias;
            EntityItem parentEI = joinEntityItem.JoinProperty.ParentEntityItem;
            string parentAlias = parentEI.FullObjectName;
            IDataEntityType joinPropertyDT = joinEntityItem.JoinProperty.ParentJoinProperty.Parent;
            if (ORMConfiguration.IsMultiLangDataEntityType(joinPropertyDT))
                parentAlias = parentAlias + "_L";
            join.JoinTableAlias = parentAlias;
            join.JoinField = baseDataAndMul ? join.Field : joinField;
            if (parentEI != this.EntityItem && !this.joinAliasSet.Contains(parentAlias))
            {
                string pproperty = parentEI.FullObjectName + "." + parentEI.EntityType.PrimaryKey.Name;
                PutField(new PropertyField(pproperty), false, false);
            }
            string tableGroup = joinEntityItem.JoinProperty.ParentJoinProperty.TableGroup;
            if (!string.IsNullOrEmpty(tableGroup))
            {
                string fullObjectName, groupAlias;
                bool isEntry = ORMConfiguration.IsEntryEntityType(parentEI.EntityType);
                if (isEntry)
                {
                    fullObjectName = parentAlias.Substring(0, parentAlias.LastIndexOf('.'));
                    groupAlias = fullObjectName + '.' + parentEI.EntityType.Name + '_' + tableGroup;
                }
                else
                {
                    fullObjectName = parentAlias;
                    groupAlias = parentAlias + '_' + tableGroup;
                }
                join.JoinTableAlias = groupAlias;
                AddJoinExtend(fullObjectName, parentEI, joinEntityItem.JoinProperty.ParentJoinProperty, tableGroup, groupAlias, isEntry);
            }
            this.JoinTableList.Add(join);
        }


        private void AddJoinExtend(string fullObjectName, EntityItem ei, IDataEntityProperty propertyType, string tableGroup, string groupAlias, bool isEntry)
        {
            string joinField, joinTableAlias;
            if (this.joinAliasSet.Contains(groupAlias.ToLower()))
                return;
            this.joinAliasSet.Add(groupAlias.ToLower());
            JoinTableInfo join = new JoinTableInfo(ei, JoinTableTypeEnum.extend);
            string tableName = (isEntry ? propertyType.Parent.Alias : ei.EntityType.Alias) + '_' + tableGroup;
            join.TableAlias = tableName;
            if (ORMHint.IsInnerJoinConfigured(join.Table))
            {
                join.Join = ORMHint.JoinHint.INNER;
            }
            else
            {
                join.Join = this.ormHint.joinHint(ORMUtil.GetFullObjNameWithoutRoot(fullObjectName));

            }
            join.JoinTableAlias = groupAlias;
            if (isEntry)
            {
                joinField = propertyType.Parent.PrimaryKey.Alias;
                joinTableAlias = ei.FullObjectName;
            }
            else
            {
                joinField = ei.EntityType.PrimaryKey.Alias;
                joinTableAlias = fullObjectName;
            }
            join.Field = joinField;
            join.JoinField = joinField;
            join.JoinTableAlias = joinTableAlias;
            this.JoinTableList.Add(join);

        }
        private PropertyField PutField(PropertyField field, bool add2SelectObjectMap, bool add2PerformJoinFieldList)
        {
            return PutField(field, add2SelectObjectMap, add2PerformJoinFieldList, ORMHint.JoinHint.DEFAULT);
        }

        public EntityItem GetEntityItem(string fullObjectName)
        {
            return this.EntityItemLoader.Load(fullObjectName.ToLower());
        }
        public string GetSimpleEntityAlias(string entityAlias)
        {
            if (this.replaceEntityAliasMap != null && this.replaceEntityAliasMap.Count > 0)
            {
                string replaceAlias = this.replaceEntityAliasMap.GetValueOrDefault(entityAlias.ToLower(), "");
                if (!string.IsNullOrEmpty(replaceAlias)) entityAlias = replaceAlias;

            }
            string simpleAlias = this.simpleEntityAliasMap.GetValueOrDefault(entityAlias.ToLower(), "");
            if (string.IsNullOrEmpty(simpleAlias))
            {
                char ch = (char)(65 + this.simpleEntityAliasMap.Count);
                if (ch <= 'Z')
                {
                    //simpleAlias = new string(new char[] { ch });
                    simpleAlias = $"{ch}";
                }
                else
                {
                    simpleAlias = "T" + $"{this.simpleEntityAliasMap.Count - 26}";
                }
                this.simpleEntityAliasMap[entityAlias.ToLower()] = simpleAlias;
            }

            return simpleAlias;
        }

    }
}
