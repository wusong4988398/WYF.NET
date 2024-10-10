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

        public static DynamicObjectCollection Query(String algoKey, String entityName, String selectFields, QFilter[] filters, String orderBys)
        {
            ORM orm = ORM.Create();
            IDataSet ds = orm.QueryDataSet(algoKey, entityName, selectFields, filters, orderBys);
            DynamicObjectCollection rows = orm.ToPlainDynamicObjectCollection(ds);
            return rows;

        }

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
    }
}
