using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.OrmEngine;
using WYF.OrmEngine.Query;
using static IronPython.Modules.PythonCsvModule;

namespace WYF.ServiceHelper
{
    public class QueryServiceHelper
    {

        public static DynamicObjectCollection Query(string entityName, string selectFields, QFilter[] filters)
        {
            return Query("QueryServiceHelper." + entityName, entityName, selectFields, filters, "");
        }


        public static DynamicObjectCollection Query(string entityName, string selectFields, QFilter[] filters, string orderBys)
        {
            return Query("QueryServiceHelper." + entityName, entityName, selectFields, filters, orderBys);
        }

        public static DynamicObjectCollection Query(String algoKey, String entityName, String selectFields, QFilter[] filters, String orderBys)
        {
            ORM orm = ORM.Create();
            IDataSet ds = orm.QueryDataSet(algoKey, entityName, selectFields, filters, orderBys);
            DynamicObjectCollection rows = orm.ToPlainDynamicObjectCollection(ds);
            return rows;

        }
        /// <summary>
        /// 查询单据（排序，前top条），返回拉平的数据包
        /// </summary>
        /// <param name="entityName">单据实体标识，如"sal_saleorder"</param>
        /// <param name="selectFields">查询字段，多个字段用逗号隔开，单据体字段需增加单据体标识做前缀，如："id, billno, entryentity.qty"</param>
        /// <param name="filters">过滤条件</param>
        /// <param name="orderBys">排序条件</param>
        /// <param name="top">查询前top条</param>
        /// <returns></returns>
        public static DynamicObjectCollection Query(string entityName, string selectFields, QFilter[] filters, string orderBys, int top)
        {
            return Query("QueryServiceHelper." + entityName, entityName, selectFields, filters, orderBys, top);
        }
        public static DynamicObjectCollection Query(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int top)
        {
            ORM orm = ORM.Create();
            IDataSet ds = orm.QueryDataSet(algoKey, entityName, selectFields, filters, orderBys, top);
            DynamicObjectCollection rows = orm.ToPlainDynamicObjectCollection(ds);
            return rows;
        }

        public static DynamicObject? QueryOne(string entityName, string selectFields, QFilter[] filters)
        {
            DynamicObjectCollection list = Query("QueryServiceHelper." + entityName, entityName, selectFields, filters, null, 1);

            return (list.Count == 0) ? null : list[0];
        }
    }
}
