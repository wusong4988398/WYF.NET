using Antlr4.Runtime.Misc;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query.crud.read;
using WYF.Bos.Orm.query.oql.g.expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public class MultiQueryBuilder
    {
        private MultiQueryParameter mp;
        private Dictionary<string, EntityTypeItem> _entityTypeMap = new Dictionary<string, EntityTypeItem>();
        public MultiQueryBuilder(MultiQueryParameter mp)
        {
            this.mp = mp;
            if (mp.SelectFields == null || mp.SelectFields.Trim().Length == 0)
                mp.SelectFields = (new SelectFieldBuilder(mp.EntityType.Name.ToLower(), mp.EntityTypeCache)).BuildSelectFields(true);

            this._entityTypeMap[mp.EntityType.Name.ToLower()] = new EntityTypeItem(mp.EntityType, null);
        }

        public MultiQuery Build()
        {
            bool mainObj;
            QueryFieldInfo queryFieldInfo = ParseQueryFieldInfo();
            string rootObjName = this.mp.EntityType.Name.ToLower();
            List<OrderByInfo> allOrderByList = OrderByInfo.GetOrderByList(this.mp.OrderBys, rootObjName);
            List<GroupByInfo> allGroupByList = GroupByInfo.GetGroupByList(this.mp.GroupBys, rootObjName);
            //MergeDBBeacon.MarkAboutHandleAllFilterAndOrderAndGroupBy();
            QContext allCtx = new QContext(this.mp.EntityType, rootObjName, queryFieldInfo.InnerSelectFields, this.mp.EntityTypeCache, this.mp.OrmHint, this.mp.Distinctable, true, new Dictionary<string, string>(), null);
            TransFilter(rootObjName, allCtx);
            Dictionary<string, List<PropertyField>> selectObjMap = allCtx.SelectFieldMap;
            Dictionary<string, QFilter> allJoinFilterMap = CreateObjFilterMap(this.mp.JoinFilters, rootObjName, allCtx, true);
            Dictionary<string, QFilter> allWhereFilterMap = CreateObjFilterMap(this.mp.WhereFilters, rootObjName, allCtx, false);
            Dictionary<string, QFilter> allHavingFilterMap = CreateObjFilterMap(this.mp.Havings, rootObjName, allCtx, false);
            allCtx.GetOrderBy((OrderByInfo[])allOrderByList.ToArray());
            allCtx.GetGroupBy((GroupByInfo[])allGroupByList.ToArray());
            String aliasRootPrefix = rootObjName.ToLower() + ".";
            List<SingleQuery> allQuery = new List<SingleQuery>(4);
            List<PropertyField> shouldSelectPKList = new List<PropertyField>();
            ISet<string> entryObjectSet = new HashSet<String>();

            Dictionary<string, string> parentObjNameMap = new Dictionary<string, string>();

            foreach (var subObjName in selectObjMap.Keys)
            {
                if (!entryObjectSet.Contains(subObjName) && !subObjName.Equals(rootObjName))
                {
                    EntityItem subEI = allCtx.GetEntityItem(subObjName);
                    string parentObjName2 = subEI.JoinProperty.ParentEntityItem.FullObjectName;
                    parentObjNameMap[subObjName] = parentObjName2;
                    List<PropertyField> plist = GetAndSetSharedProperties(selectObjMap, parentObjName2, entryObjectSet, allCtx);
                    string withoutRootSubObjName = subObjName;
                    if (withoutRootSubObjName.StartsWith(rootObjName))
                    {
                        withoutRootSubObjName = withoutRootSubObjName.Substring(rootObjName.Length + 1);
                    }
                    if (subEI.JoinProperty.IsIJoinProperty)
                    {
                        shouldSelectPKList.Add(AddQueryPrimaryKey(subObjName, withoutRootSubObjName, subEI.JoinProperty.SubJoinProperty.Name, true, plist, allCtx));
                        shouldSelectPKList.Add(AddQueryPrimaryKey(parentObjName2, withoutRootSubObjName, subEI.JoinProperty.ParentJoinProperty.Name, true, plist, allCtx));
                    }
                    else
                    {
                        shouldSelectPKList.Add(AddQueryPrimaryKey(parentObjName2, withoutRootSubObjName, subEI.JoinProperty.ParentOriginProperty.Name, true, plist, allCtx));
                    }
                }
            }

            foreach (var entry in selectObjMap)
            {
                string objName = entry.Key;
                if (!entryObjectSet.Contains(objName))
                {
                    List<PropertyField> plist2 = entry.Value;
                    if (!objName.Equals(rootObjName))
                    {
                        mainObj = false;
                        shouldSelectPKList.Add(AddQueryPrimaryKey(objName, "", allCtx.GetEntityItem(objName).EntityType.PrimaryKey.Name.ToLower(), !this.mp.ShouldSelectPK, plist2, allCtx));
                    }
                    else
                    {
                        mainObj = true;
                    }
                    PropertyField[] properties = (PropertyField[])plist2.ToArray();
                    foreach (PropertyField pf in properties)
                    {
                        if (pf.Alias.ToLower().StartsWith(aliasRootPrefix))
                        {
                            pf.Alias= pf.Alias.Substring(aliasRootPrefix.Length);
                        }
                    }
                    EntityTypeItem typeItem = GetEntityType(objName);
                    if (typeItem.ParentFK != null)
                    {
                        bool useAlias = false;
                        String parentObjName3 = parentObjNameMap.GetValueOrDefault(objName);
                        if (parentObjName3 == null)
                        {
                            
                            parentObjName3 = objName.Substring(0, objName.LastIndexOf('.'));
                        }
                        List<PropertyField> it2= selectObjMap.GetValueOrDefault(parentObjName3);
                        foreach (PropertyField p in it2)
                        {
                            
                            if (typeItem.ParentFK.Equals(p.Name,StringComparison.OrdinalIgnoreCase))
                            {
                                useAlias = true;
                                typeItem.ParentFK = p.Alias;
                                if (typeItem.ParentFK.IndexOf('.') != -1 && p.Alias.Equals(p.GetFullName()))
                                {
                                    typeItem.ParentFK = p.PropertyItem.PropertyName;
                                }
                            }
                        }
                
                    
                        if (!useAlias)
                        {
                            String joinObjName = ORMUtil.GetFullObjNameWithoutRoot(objName);
                            int dot2 = joinObjName.LastIndexOf(".");
                            if (dot2 != -1)
                            {
                                typeItem.ParentFK = joinObjName.Substring(0, dot2 + 1) + typeItem.ParentFK;
                            }
                        }
                    }
                    SingleQuery subQuery = new SingleQuery(this.mp.DbRoute, typeItem.ParentFK, typeItem.Type, objName, properties, allCtx.GetPerformJoinFieldList(objName), allJoinFilterMap, allWhereFilterMap, allOrderByList, allGroupByList, allHavingFilterMap, mainObj ? this.mp.Top : -1, this.mp.Start, this.mp.Limit, allCtx, false, false, true);

                    subQuery.WithUserOriginFilter = subQuery.WithWhereFilter();
                    allQuery.Add(subQuery);
                }
            }

            allQuery.Sort((o1, o2) =>
            {
                string n1 = o1.FullObjName;
                if (n1.Equals(rootObjName, StringComparison.OrdinalIgnoreCase))
                {
                    return -1;
                }
                string n2 = o2.FullObjName;
                if (n2.Equals(rootObjName, StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }
                return n1.CompareTo(n2);
            });
            if (this.mp.ShouldSelectPK)
            {
                List<PropertyField> buf = new List<PropertyField>(queryFieldInfo.UserSelectFields);
                foreach (PropertyField pf2 in shouldSelectPKList.Where(pf => pf != null && !buf.Any(pf1 => pf1.IsSameWith(pf))))
                {
                    buf.Add(pf2);
                }
                if (buf.Count > queryFieldInfo.UserSelectFields.Length)
                {
                    queryFieldInfo.UserSelectFields = buf.ToArray();
                }
            }
            return new MultiQuery(allQuery.ToArray(), queryFieldInfo.UserSelectFields, this.mp.Optimization, allCtx);
        }
        private PropertyField AddQueryPrimaryKey(string objName, string withoutRootObjName, string propertyName, bool asInnerPK, List<PropertyField> ps, QContext ctx)
        {
            String alias;
            if (asInnerPK)
            {
                String suffix;
                if (withoutRootObjName.Length == 0)
                {
                    suffix = propertyName;
                }
                else
                {
                    suffix = withoutRootObjName.Replace('.', '_');
                }
                alias = "INNER_" + suffix;
            }
            else
            {
                alias = null;
            }

            PropertyField sf = new PropertyField(objName, propertyName, alias);
            bool exist = false;
            foreach (PropertyField p in ps)
            {
                if (p.IsSameWith(sf))
                    exist = true;
            }
     
            if (!exist)
            {
                sf = ctx.AddSelectField(sf);
                return asInnerPK ? null : sf;
            }
            return null;
        }

        private List<PropertyField> GetAndSetSharedProperties(Dictionary<string, List<PropertyField>> selectObjMap, string objName, ISet<string> entryObjectSet, QContext ctx)
        {
            string mainObjName = objName;
            ISet<String> pathObjSet = new HashSet<String>(2);
            pathObjSet.Add(objName);
            while (true)
            {
                EntityItem ei = ctx.GetEntityItem(mainObjName);
                if (ORMConfiguration.IsEntryEntityType(ei.EntityType))
                {
                    entryObjectSet.Add(mainObjName);
                    mainObjName = ORMUtil.GetParentObjectName(mainObjName);
                    pathObjSet.Add(mainObjName);
                    continue;
                }
                break;
            }

            
            List<PropertyField> pfList = selectObjMap.GetValueOrDefault(mainObjName);
            if (pfList == null)
                pfList = new List<PropertyField>();
            foreach (string obj in pathObjSet)
            {
                selectObjMap[obj] = pfList;
            
            }

            return pfList;
       
        
        }

        private Dictionary<string, QFilter> CreateObjFilterMap(QFilter[] filters, string rootObjName, QContext ctx, bool isJoinFilters)
        {
            Dictionary<string, QFilter> filterMap = new Dictionary<string, QFilter>();
            if (filters != null && filters.Length > 0)
            {
                foreach (QFilter filter in filters)
                {
                    string objName;
                    filter.ToQParameter(ctx);
                }
            }
            return null;
        }

        private void TransFilter(string rootObjName, QContext allCtx) {
            
            QFilter[][] allFilters = { this.mp.JoinFilters, this.mp.WhereFilters, this.mp.Havings };
            QFilterJoinSQLTransFunction sqltf = new QFilterJoinSQLTransFunction();
            foreach (QFilter[] filters in allFilters)
                DoTransFilter(filters, t => { return sqltf.SQLTransFunction(t); });
            

        }

        private void DoTransFilter(QFilter[] filters, Func<QFilter, QFilter> tf)
        {
            if (filters != null)
            {
                for (int i = 0, n = filters.Length; i < n; i++)
                    filters[i] = filters[i].Trans(tf);
            }
                
        }
        private QueryFieldInfo ParseQueryFieldInfo()
        {
            List<PropertyField> pfs;
            string rootObjName = this.mp.EntityType.Name.ToLower();
            QueryFieldInfo ret = new QueryFieldInfo();
            try
            {
                pfs = SelectFields.ParseFrom(this.mp.SelectFields).CreatePropertyFields(rootObjName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(rootObjName + " select field error: " + this.mp.SelectFields, ex);
            }
            List<string> newFields = new ArrayList<string>();
            foreach (PropertyField field in pfs)
            {
                if ("*"== field.Name)
                {
                    string addedObjPrefix, aliasPrefix = field.Alias;
                    if (aliasPrefix.IndexOf('*') != -1)
                        aliasPrefix = "";
                    EntityTypeItem eti = GetEntityType(field.FullObjectName);
                    addedObjPrefix = string.IsNullOrEmpty(field.FullObjectName) ? rootObjName + "." : rootObjName + "." + field.FullObjectName.ToLower() + ".";
                    DataEntityPropertyCollection ps = eti.Type.Properties;
                    CollectAutoSelectFields(addedObjPrefix, aliasPrefix, ps, newFields);
                    pfs.Remove(field);

                }
            }
            foreach (string newField in newFields)
            {
                PropertyField pf = new PropertyField(newField.Trim());
                pfs.Add(pf);
            }

            ret.InnerSelectFields = pfs.ToArray();
            ret.UserSelectFields = pfs.ToArray();
            return ret;

        }


        private void CollectAutoSelectFields(string addedObjPrefix, string aliasPrefix, DataEntityPropertyCollection ps, List<string> newFields)
        {
            for (int j = 0, n = ps.Count; j < n; j++)
            {
                IDataEntityProperty dp = (IDataEntityProperty)ps[j];
                if (dp is ICollectionProperty)
                {

                }else if (!(dp is IComplexProperty))
                {
                    if (!ORMUtil.IsDbIgnore(dp))
                    {
                        string name = dp.Name.ToLower();
                        if (!name.EndsWith("_id") || !ps.ContainsKey(name.Substring(0, name.Length - 3)))
                        {
                            string added = addedObjPrefix + dp.Name;
                            if (!string.IsNullOrEmpty(aliasPrefix)) added = added + " " + aliasPrefix + dp.Name;
                            if (newFields.IndexOf(added) == -1)
                            {
                                newFields.Add(added);
                            }

                        }
                    }
                }

            
            }
        }

        private EntityTypeItem GetEntityType(string objName)
        {
            if (string.IsNullOrEmpty(objName)) objName = this.mp.EntityType.Name;
            objName = objName.ToLower();
            EntityTypeItem typeItem = this._entityTypeMap.GetValueOrDefault(objName,null);
            if (typeItem == null)
            {
                string parentObjName, subObj;
                int pos = objName.LastIndexOf('.');
                if (pos != -1)
                {
                    parentObjName = objName.Substring(0, pos);
                    subObj = objName.Substring(pos + 1);
                }
                else
                {
                    parentObjName = this.mp.EntityType.Name.ToLower();
                    subObj = objName;
                }
                IDataEntityType parentType = (GetEntityType(parentObjName)).Type;
                IDataEntityProperty fk = GetProperty(parentType, subObj);
                if (fk is ICollectionProperty){
                    IDataEntityType type = ((ICollectionProperty)fk).ItemType;
                    typeItem = new EntityTypeItem(type, subObj);
                    this._entityTypeMap[objName]= typeItem;
                }
                else if (fk is IComplexProperty) {
                    IDataEntityType type = ORMConfiguration.InnerGetBaseDataEntityType((IComplexProperty)fk, this.mp.EntityTypeCache);
                    typeItem = new EntityTypeItem(type, subObj);
                    this._entityTypeMap[objName] = typeItem;
                }
                else
                {
                    try
                    {
                        IDataEntityType type = ORMConfiguration.InnerGetDataEntityType(subObj, this.mp.EntityTypeCache);
                        typeItem = new EntityTypeItem(type, subObj);
                        this._entityTypeMap[objName] = typeItem;
                    }
                    catch (Exception ex)
                    {

                        throw new Exception($"{parentType.Name}属性不存在或未设置字段：{subObj}");
                    }
                    
                }


                
            }
            return typeItem;
        }

        private IDataEntityProperty GetProperty(IDataEntityType type, string property)
        {

            IDataEntityProperty ret = (IDataEntityProperty)type.Properties[property];
            if (ret == null)
            {
                property = property.ToLower();
                DataEntityPropertyCollection cc = type.Properties;
                for (int i = 0, n = cc.Count; i < n; i++)
                {
                    IDataEntityProperty p = (IDataEntityProperty)cc[i];
                    string name = p.Name.ToLower();
                    if (property== name)
                        return p;
                }
            }
            return ret;
        }

        private  class QueryFieldInfo
        {
            public PropertyField[] InnerSelectFields { get; set; }

            public PropertyField[] UserSelectFields { get; set; }




            public QueryFieldInfo() { }
        }

        private class EntityTypeItem
        {
            private string _parentFK;

            public string ParentFK { get { return _parentFK; } set { _parentFK = value; } }

            private IDataEntityType _type;
            public IDataEntityType Type
            {
                get { return this._type; }

            }
            public EntityTypeItem(IDataEntityType type, String parentFK)
            {
                this._parentFK = parentFK;
                this._type = type;
            }
        }
    }


}
