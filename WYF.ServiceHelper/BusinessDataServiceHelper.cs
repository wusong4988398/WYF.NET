using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Data;
using WYF.OrmEngine;
using WYF.OrmEngine.Query;
using WYF.Algo;
using System.Text.RegularExpressions;
using IronPython.Runtime.Operations;
using WYF.Entity;
using System.Collections;
using WYF.OrmEngine.dataManager;
using WYF.DataEntity.Metadata;

namespace WYF.ServiceHelper
{
    /// <summary>
    /// 数据操作服务类，提供查询、保存等功能
    /// </summary>
    public class BusinessDataServiceHelper
    {
        private static readonly DynamicObject[] EmptyObjects = new DynamicObject[0];
        public static IDictionary<object, DynamicObject> LoadFromCache(string entityName, string selectProperties, QFilter[] filters, string orderBy)
        {
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
            List<object> idList = new List<object>();
            if (orderBy.IsNullOrWhiteSpace())
            {
                using (IDataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, "id", filters, null, -1,WithEntityEntryDistinctable.Instance))
                {
                    idList.AddRange(from row in ds select row.Get(0));
                }
            }
            else
            {
                // 创建IDSet实例
                //IDSet idSet = new IDSet();
                // 初始化selectFields
                string selectFields = "id";
                // 使用正则表达式进行匹配
                if (Regex.IsMatch(orderBy, @"(^id$)|(^id[ ,]+)|([ ,]+id[ ,]+)|([ ,]+id$)"))
                {
                    selectFields = Regex.Replace(orderBy.ToLower(), @"\basc\b|\bdesc\b", "");
                }
                else
                {
                    selectFields += "," + Regex.Replace(orderBy.ToLower(), @"\basc\b|\bdesc\b", "");
                }
                using (IDataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, selectFields, filters, orderBy, -1))
                {
                    idList.AddRange(from row in ds select row.Get(0));
                }
            }
            IDictionary<object, DynamicObject> fromCache = BusinessDataReader.LoadFromCache(idList.ToArray(), type);
            IDictionary<object, DynamicObject> listObjs = new Dictionary<object, DynamicObject>();
            foreach (var id in idList)
            {
                DynamicObject tObj = fromCache.GetOrDefault(id);
                if (null != tObj)
                    listObjs[id]=tObj;
            }
            //DataEntityCacheManager cacheManager = new DataEntityCacheManager(type);
            return listObjs;



        }
        public static DynamicObject LoadSingleFromCache(string entityName, string selectProperties, QFilter[] filters)
        {
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);

            return LoadSingleFromCache(filters, type);
        }

        public static DynamicObject LoadSingle(object pk, DynamicObjectType type)
        {
            return BusinessDataReader.LoadSingle(pk, type);
        }

        private static DynamicObject LoadSingleFromCache(QFilter[] filters, DynamicObjectType type)
        {

            List<object> idList = new List<object>();
            DataEntityCacheManager cacheManager = new DataEntityCacheManager((IDataEntityType)type);
            object[] pks = cacheManager.GetCachePks(filters);
            if (pks == null)
            {
                using (IDataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.LoadFromCache", type.Name, "id", filters, null, -1, WithEntityEntryDistinctable.Instance))
                {
                    idList.AddRange(from row in ds select row.Get(0));

                }
                pks = idList.ToArray();
                cacheManager.PutCachePks(filters, pks);
            }

            IDictionary<object, DynamicObject> mapObject = BusinessDataReader.LoadFromCache(pks, type);
           
            return (mapObject.Count > 0) ? mapObject.Values.ToArray()[0] : null;
        }
        public static DynamicObject[] Load(String entityName, String selectProperties, QFilter[] filters)
        {
            return Load(entityName, selectProperties, filters, null, -1);
        }
        /// <summary>
        /// 按条件读取实体数据，取top张
        /// </summary>
        /// <param name="entityName">实体标识</param>
        /// <param name="selectProperties">读取字段：使用逗号分隔字段标识，单据体字段要带单据体标识，格式如："id, bill, entryentity.qty"</param>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="top">取前几条</param>
        /// <returns></returns>
        public static DynamicObject[] Load(String entityName, String selectProperties, QFilter[] filters, String orderBy, int top)
        {

            if (top == 0) return EmptyObjects;
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
            List<object> idList = new List<object>();
            if (orderBy.IsNullOrEmpty())
            {
                
                using (IDataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, "id", filters, null, top,
                         WithEntityEntryDistinctable.Instance))

                {
                    foreach (var row in ds)
                    {
                        idList.Add(row.Get(0));
                    }
                }
                return BusinessDataReader.Load(idList.ToArray(), type, true);
            }

            
            string selectFields = "id";
            if (Regex.IsMatch(orderBy, @"(^id$)|(^id[ ,]+)|([ ,]+id[ ,]+)|([ ,]+id$)"))
            {
                selectFields = Regex.Replace(orderBy.ToLower(), @"\s*asc|\s*desc", "");
            }
            else
            {
                selectFields = selectFields + "," + Regex.Replace(orderBy.ToLower(), @"\s*asc|\s*desc", "");
            }

            using (IDataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, "id", filters, selectFields, top,
                     WithEntityEntryDistinctable.Instance))

            {
                if (top<0)
                {
                    foreach (var row in ds)
                    {
                        idList.Add(row.Get("id"));
                    }
                }
                else
                {
                    while (ds.HasNext())
                    {

                        IRow row = ds.Next();
                        idList.Add(row.Get("id"));
                        if (idList.Count()==top)
                        {
                            break;
                        }
                    }
                }


            
            }
            DynamicObject[] objs = BusinessDataReader.Load(idList.ToArray(), type, true);
            return OrderBy(objs, idList, orderBy);
        }



        private static DynamicObjectType GetSubEntityType(string entityName, string selectProperties)
        {
            // 将 selectProperties 按逗号分隔，并去除每个属性的空白字符
            string[] properties = selectProperties.Split(',');
            HashSet<string> select = new HashSet<string>(properties.Length);
            foreach (string prop in properties)
            {
                select.Add(prop.Trim());
            }
            // 调用 EntityMetadataCache 的方法获取子实体类型
            return EntityMetadataCache.GetSubDataEntityType(entityName, select);
        }
    

    private static DynamicObject[] OrderBy(DynamicObject[] objs, List<object> idList, string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy) || idList.Count <= 1)
                return objs;

            // 创建一个字典来存储对象，键是主键值
            Dictionary<object, DynamicObject> maps = new Dictionary<object, DynamicObject>();
            foreach (DynamicObject o in objs)
            {
                maps[o.PkValue] = o;
            }
            // 根据 idList 的顺序创建一个新的列表
            List<DynamicObject> listDyn = new List<DynamicObject>();
            foreach (object id in idList)
            {
                if (maps.TryGetValue(id, out DynamicObject o))
                {
                    listDyn.Add(o);
                }
            }

            // 将列表转换为数组并返回
            return listDyn.ToArray();
        }
    

    //public static DynamicObject[] Load(string entityName, string selectProperties, QFilter[] filters, string orderBy, int pageIndex, int pagesize)
    //{
    //    if (pageIndex < 0 || pagesize <= 0)
    //        return EmptyObjects;
    //    DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
    //    if (orderBy.IsNullOrEmpty())
    //    {
    //        List<object> idList = new List<object>();
    //        DataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, "id", filters, null, pageIndex * pagesize, pagesize,
    //             (entityType, joinEntitySelectFieldMap) =>
    //             {
    //                 string entityName = entityType.Name;
    //                 int len = entityName.Length;
    //                 foreach (KeyValuePair<string, bool> entry in joinEntitySelectFieldMap)
    //                 {
    //                     string entryFullName = entry.Key;
    //                     if (entryFullName.Length <= len)
    //                         continue;
    //                     string entryName = entryFullName.Substring(len + 1);
    //                     if (!entryName.Contains("."))
    //                     {

    //                         IDataEntityProperty dp = entityType.Properties[entryName];
    //                         if (dp is ICollectionProperty)
    //                         {
    //                             IDataEntityType type = ((ICollectionProperty)dp).ItemType;
    //                             if (ORMConfiguration.IsEntryEntityType(type))
    //                                 if (!entry.Value)
    //                                     return true;
    //                         }
    //                     }
    //                 }
    //                 return false;
    //             }
    //            );
    //        return BusinessDataReader.Load(idList.ToArray(), type, true);
    //    }

    //}

    public static DynamicObject[] Load(string entityName, string selectProperties, QFilter[] filters, string orderBy, int pageIndex, int pagesize)
        {
            if (pageIndex < 0 || pagesize <= 0)
                return EmptyObjects;
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
            if (orderBy.IsNullOrEmpty())
            {
                List<object> idList = new List<object>();
                IDataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, "id", filters, null, pageIndex * pagesize, pagesize,
                     WithEntityEntryDistinctable.Instance
                    );
                //foreach (DataRow row in ds.Tables[0].Rows)
                //{
                //    idList.Add(row[0]);
                //}
                //return BusinessDataReader.Load(idList.ToArray(), type, true);
            }
            return null;

        }

        internal DynamicObject[] Load(string formId, List<SelectorItemInfo> selector, OQLFilter ofilter)
        {
            return null;
        }
        internal DynamicObject[] Load(string entityName, string selectProperties, OQLFilter ofilter)
        {
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
            return null;
        }

   

    }
}
