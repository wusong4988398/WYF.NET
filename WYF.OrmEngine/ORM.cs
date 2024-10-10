using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.OrmEngine.Impl;
using WYF.OrmEngine.Query;

namespace WYF.OrmEngine
{
    public interface ORM
    {
        static ORM Create()
        {
            return (ORM)new ORMImpl();
        }
        IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys);
        IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, String orderBys, int top);
        IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int top, IDistinctable distinctable);
        IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, IDistinctable distinctable);
  
        //IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, IDistinctable distinctable);
        //IDataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, String orderBys, int top);

        DynamicObjectCollection ToPlainDynamicObjectCollection(IDataSet ds);
        //DataSet QueryDataSet(string algoKey, string entityName, string selectFields, QFilter[] filters, string orderBys, int from, int length, Func<IDataEntityType, Dictionary<string, bool>, bool> distinctable);
    }
}
