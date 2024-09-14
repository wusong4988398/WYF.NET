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

namespace WYF.ServiceHelper
{
    /// <summary>
    /// 业务数据服务
    /// </summary>
    public class BusinessDataServiceHelper
    {
        private static readonly DynamicObject[] EmptyObjects = new DynamicObject[0];
        public static Dictionary<object, DynamicObject> LoadFromCache(string entityName, string selectProperties, QFilter[] filters, string orderBy)
        {
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
            List<object> idList = new List<object>();
            //DataEntityCacheManager cacheManager = new DataEntityCacheManager(type);


            return null;



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
        //             (entityType, joinEntitySelectFieldMap) => {
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
        //   }

        //}

        public static DynamicObject[] Load(string entityName, string selectProperties, QFilter[] filters, string orderBy, int pageIndex, int pagesize)
        {
            if (pageIndex < 0 || pagesize <= 0)
                return EmptyObjects;
            DynamicObjectType type = GetSubEntityType(entityName, selectProperties);
            if (orderBy.IsNullOrEmpty())
            {
                List<object> idList = new List<object>();
                DataSet ds = ORM.Create().QueryDataSet("BusinessDataServiceHelper.load", entityName, "id", filters, null, pageIndex * pagesize, pagesize,
                     WithEntityEntryDistinctable.Instance
                    );
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    idList.Add(row[0]);
                }
                return BusinessDataReader.Load(idList.ToArray(), type, true);
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

        private static DynamicObjectType GetSubEntityType(string entityName, string selectProperties)
        {
            string[] properties = selectProperties.Split(",");
            ISet<string> select = new HashSet<string>(properties.Length);

            foreach (string property in properties)
            {
                select.Add(property.Trim());
            }

            //return EntityMetadataCache.GetSubDataEntityType(entityName, select);
            return null;


        }

    }
}
